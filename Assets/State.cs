using System.Collections;

public abstract class State
{
    protected EnemyAI _enemyAI;

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Idle()
    {
        yield break;
    }

    public virtual IEnumerator MoveToGoal()
    {
        yield break;
    }

    public virtual IEnumerator Search()
    {
        yield break;
    }

    public virtual IEnumerator Chase()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

}
