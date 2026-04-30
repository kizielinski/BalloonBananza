using UnityEngine;

public abstract class Boss : Enemy
{
    [SerializeField] private string bossName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bossName == "")
        {
            throw new System.Exception("This boss isn't intialized! Make sure it has a name the player remembers! Example: Octhor the Impaler");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void BossArise(); //Fades/Moves boss in and activates on screen Boss HUD
    public abstract void Attack(); //4-Cycle Attack: Dive @ Player, Recover (X Time), Locate Player (Aim), Repeat


}
