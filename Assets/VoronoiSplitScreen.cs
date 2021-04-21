using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class VoronoiSplitScreen : MonoBehaviour
{
    #region Structs

    /// <summary>
    /// Stores the rendering properties required to store for processing the screen.
    /// </summary>
    private struct RenderProperties
    {
        public readonly int Width;
        public readonly int Height;
        public readonly float AspectRatio;
        public float OrthoSize;

        public RenderProperties(int _width, int _height, float orthoSize = 0)
        {
            Width = _width;
            Height = _height;
            AspectRatio = (float)_width / _height;
            OrthoSize = orthoSize;
        }
    }
    #endregion

    #region Constants

    private readonly string[] SHADER_PLAYER_POSITION = new[]
    {
        "_playerPos1",
        "_playerpos2",
        "_playerPos3",
        "_playerPos4"
    };

    private readonly string SHADER_PLAYER = "_player";
    private readonly string SHADER_LINE_COLOR = "_lineColor";
    private readonly string SHADER_LINE_THICKNESS = "_lineThickness";
    private readonly string SHADER_CELLS_STENCIL_OP = "_VoronoiCellsStencilOp";
    private readonly string SHADER_CELLS_STENCIL_PLAYER = "_VoronoiCellsPlayerStencil";
    private readonly string SHADER_CELLS_STENCIL_TEX = "_VornoiTex";
    private readonly string SHADER_MASKED_STENCIL_OP = "_MaskedStencilOp";
    private readonly string SHADER_BLEND_TEXTURE = "_SecondaryTex";

    private const int MAX_PLAYERS = 4;

    #endregion

    #region Inspector panel variables
    [Header("Camera References")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private Camera[] _playerCameras;
    [SerializeField]
    private Renderer _maskRenderer;
    [SerializeField]
    private Transform _maskTransform;

    [Header("Materials")]
    [SerializeField]
    private Material _voronoiCellsMaterial;
    [SerializeField]
    private Material _splitLineMaterial;
    [SerializeField]
    private Material _alphaBlendMaterial;
    [SerializeField]
    private Material _fxaaMaterial;

    [Header("Graphics")]
    [SerializeField]
    private Color _lineColor = Color.black;
    [SerializeField]
    private bool _enableFXAA = true;

    [Header("Players")]
    [SerializeField]
    private int _playerCount = 0;
    [SerializeField]
    private Transform[] _players;
    /// <summary>
    /// Enables the cameras to merge together if players get too close to eachother.
    /// </summary>
    [SerializeField]
    private bool _enableMerging = true;
    #endregion

    #region Internal variables

    private Color _lastLineColor = Color.black;

    private RenderProperties _screen;
    private RenderTexture _playerTex;
    private RenderTexture _cellsTexture;

    private Vector2[] _worldPositions = new Vector2[MAX_PLAYERS];
    private Vector2[] _normalizedPositions = new Vector2[MAX_PLAYERS];
    private Vector2 _mergedPosition = Vector2.one / 2;
    private float mergeRatio = 1f;
    private int _activePlayers = 2;

    #endregion

    private void Awake()
    {
        _voronoiCellsMaterial = Instantiate(_voronoiCellsMaterial);
        _splitLineMaterial = Instantiate(_splitLineMaterial);
        _alphaBlendMaterial = Instantiate(_alphaBlendMaterial);
        _fxaaMaterial = Instantiate(_fxaaMaterial);

        _maskRenderer.sharedMaterial = _voronoiCellsMaterial;

        InitializeCameras();

        SetLineColor(_lineColor);
    }

    private void InitializeCameras()
    {
        foreach(Camera _playerCamera in _playerCameras)
        {
            _playerCamera.depthTextureMode = DepthTextureMode.Depth;
            UpdateRenderProperties();
        }
    }

    private void UpdateRenderProperties()
    {
        OnResolutionChanged(Screen.width, Screen.height);
        OnOrthoSizeChanged(_mainCamera.orthographicSize);
    }

    private void OnResolutionChanged(int _width, int _height)
    {
        if((_screen.Width == _width) && (_screen.Height == _height))
        {
            return;
        }

        _playerTex?.Release();
        _cellsTexture?.Release();

        _playerTex = new RenderTexture(_width, _height, 32);
        _playerTex.name = "Player Render";

        foreach(Camera _playerCamera in _playerCameras)
        {
            _playerCamera.targetTexture = _playerTex;
        }

        _cellsTexture = new RenderTexture(_width, _height, 0, GraphicsFormat.R8_UNorm);
        _cellsTexture.name = "Cells Visualization Texture";

        _splitLineMaterial.SetTexture(SHADER_CELLS_STENCIL_TEX, _cellsTexture);
        _splitLineMaterial.SetFloat(SHADER_LINE_THICKNESS, (float)_height / 200);

        _screen = new RenderProperties(_width, _height);
    }

    private void OnOrthoSizeChanged(float orthoSize)
    {
        if(Mathf.Abs(_screen.OrthoSize - orthoSize) < Mathf.Epsilon)
        {
            return;
        }

        foreach(Camera _playerCamera in _playerCameras)
        {
            _playerCamera.orthographicSize = orthoSize;
        }
        _maskTransform.localScale = new Vector3(orthoSize * _screen.AspectRatio * 2, orthoSize * 2);

        _screen.OrthoSize = orthoSize;
    }

    private void SetLineColor(Color color)
    {
        _splitLineMaterial.SetColor(SHADER_LINE_COLOR, _lineColor);
        _lastLineColor = _lineColor;
    }

    private void Update()
    {
        if(_players.Length < _playerCount)
        {
            _playerCount = _players.Length;
            Debug.LogWarningFormat(
                "PlayerCount ({0}) is higher than number of players in Players ({1}) array. Setting PlayerCount to {1}.",
                _playerCount, _players.Length);
        }

        if(_playerCount > MAX_PLAYERS)
        {
            _playerCount = MAX_PLAYERS;
            Debug.LogWarningFormat(
                    "Voronoi split screen doesn't support more than {0} players right now. Setting PlayerCount to {0}",
                    MAX_PLAYERS);
        }

        if(_players.Length == 0)
        {
            _worldPositions[0] = transform.position;
        }
        for(int i = 0; i < _players.Length && i < MAX_PLAYERS; i++)
        {
            _worldPositions[i] = _players[i].position;
        }

        if(_playerCount <= 1)
        {
            _normalizedPositions[0] = Vector3.one / 2;
            mergeRatio = 0;
            _activePlayers = 1;
            RenderPlayers(_activePlayers);
            return;
        }

        UpdateRenderProperties();
        if(_lastLineColor != _lineColor)
        {
            SetLineColor(_lineColor);
        }

        Vector2 min, max;

        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            _normalizedPositions[i] = _worldPositions[i];
        }

        min = _normalizedPositions[0];
        max = _normalizedPositions[0];

        for (int i = 1; i < MAX_PLAYERS; i++)
        {
            if (_normalizedPositions[i].x < min.x) min.x = _normalizedPositions[i].x;
            if (_normalizedPositions[i].x > max.x) max.x = _normalizedPositions[i].x;
            if (_normalizedPositions[i].y < min.y) min.y = _normalizedPositions[i].y;
            if (_normalizedPositions[i].y > max.y) max.y = _normalizedPositions[i].y;
        }

        max -= min;
        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            _normalizedPositions[i] -= min;
        }

        var diff = Vector2.zero;
        if(max.x > max.y * _screen.AspectRatio)
        {
            diff.y = ((max.x / _screen.AspectRatio) - max.y) / 2;
        }
        else if(max.y > max.x * 1 / _screen.AspectRatio)
        {
            diff.x = ((max.y * _screen.AspectRatio) - max.x) / 2;
        }

        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            _normalizedPositions[i] += diff;
        }
        max += diff * 2;

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            _normalizedPositions[i] /= max;
        }


    }
}


