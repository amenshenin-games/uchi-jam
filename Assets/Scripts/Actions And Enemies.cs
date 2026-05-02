

using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Actions
{
    Attack,
    Poison,
    SuperBite,
    Confusion,
    Block,
    Trap,
    Heal,
    Parry,
    Stunning,
    Vitamin,
    DoubleBite,
    FullHeal,
    Status,
    IncreaseDamage,
    DoNothing
}


[Serializable]
public enum Tags 
{
    Active,
    Passive,
    Fly,
    Swim,
    Venom,
    Damage,
    Health,
    Defense,
    Status,
    Attack
}


[Serializable]
public class GenericEnemy : Enemy
{
    /// <summary>
    /// Класс, позволяющий сериализовать противников
    /// </summary>
    private int currAbility = 0;
    public int id; 
    public int Health; 
    public List<string> ActionStrList;
    public  List<int> Strength;
    public string Image; 
    public int ChallengeRating;

    public GenericEnemy(int id, int Health, List<string> ActionStrList, List<int> Strength, string Image, int ChallengeRating) : base(Health)
    {
        this.id = id;
        this.Health = Health;
        this.ActionStrList = ActionStrList;
        this.Strength = Strength;
        this.Health = Health;
        this.Image = Image;
        this.ChallengeRating = ChallengeRating;
        Init();
    }

    public void Init()
    {
        this.MaxHealth = Health;
        this.CurrentHealth = Health;
        this.statusEffects = new Dictionary<string, Status> ();
        abilityList = new List<Ability>();
        int i = 0;
        foreach(string actionInStr in ActionStrList)
        {
            Actions action;
            Enum.TryParse(actionInStr, out action);

            Ability a;
            switch (action)
            {
                case Actions.Attack:
                    a = new AttackAbility(Strength[i], 0);
                    break;
                case Actions.Poison:
                    a = new StatusAbility<Poisoned>(Strength[i], 0, 1);
                    break;
                case Actions.SuperBite:
                    a = new SuperBiteAbility(Strength[i], 0);
                    break;
                case Actions.Confusion:
                    a = new StatusAbility<Confused>(Strength[i], 0, 1);
                    break;
                default: // Optional fallback
                    a = new AttackAbility(Strength[i], 0);
                    break;
            }
            
            abilityList.Add(a);
            i++;
        }
    }

    /// <summary>
    /// Обычный противник просто выбирает случайное из списка своих действий.
    /// </summary>
    public override void TakeTurn(params object[] args)
    {
        abilityList[currAbility].ExecAbility(target, args);
        currAbility++;
        if (currAbility >= abilityList.Count)
        {
            currAbility = 0;
        }
    }
}

public class AttackAbility : Ability
{
    public AttackAbility(int value, int cost): base(value, cost)
    {
        name = Actions.Attack;
        tags.Add(Tags.Active);
        tags.Add(Tags.Attack);
        tags.Add(Tags.Damage);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.TakeDamage(value);
    }
}
public class Poisoned : Status
{
    public Poisoned() : base()
    {
        name = Actions.Status;
        statusName = "Poisoned";
        tags.Add(Tags.Status);
        tags.Add(Tags.Damage);
    }
    public Poisoned(int value, int cost, int duration): base(value, cost, duration)
    {
        name = Actions.Status;
        statusName = "Poisoned";
        tags.Add(Tags.Status);
        tags.Add(Tags.Damage);
        tags.Add(Tags.Active);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.TakeDamage(value);
    }

    private static List<Tags> tagList()
    {
        List<Tags> tags = new List<Tags>();
        return tags;
    }
    public override void EndOfStatus(Entity target, params object[] args)
    {
    }
}
public class StatusAbility<T>: Ability where T : Status, new()
{
    int duration;
    public StatusAbility(int value, int cost, int duration): base(value, cost)
    {
        tags.Add(Tags.Active);
        tags.Add(Tags.Status);
        this.duration = duration;
    }
    protected override void DoStuff(Entity target, params object[] args)
    {
        T pStatus = new T();
        pStatus.Init(value, 0, duration);
        Debug.Log("pStatus.statusName");
        Debug.Log(pStatus.statusName);
        if (!target.statusEffects.ContainsKey(pStatus.statusName))
        {
            target.statusEffects.Add(pStatus.statusName, pStatus);
        }
        else
        {
            target.statusEffects[pStatus.statusName].duration += 1;
        }
    }
}



public class SuperBiteAbility : Ability
{
    public SuperBiteAbility(int value, int cost): base(value, cost)
    {
        name = Actions.SuperBite;
        tags.Add(Tags.Active);
        tags.Add(Tags.Attack);
        tags.Add(Tags.Damage);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        if (value >= UnityEngine.Random.Range(1, 101))
        {   
            target.TakeDamage(target.MaxHealth / 2);
        }
    }
}

public class Confused : Status
{
    private Entity oldTarget = null;
    public Confused() : base()
    {
        name = Actions.Status;
        statusName = "Confused";
        tags.Add(Tags.Status);
        
    }
    public Confused(int value, int cost, int duration): base(value, cost, duration)
    {
        name = Actions.Status;
        statusName = "Confused";
        tags.Add(Tags.Status);
    }
    protected override void DoStuff(Entity target, params object[] args)
    {
        Enemy targetOfThisStatus = (Enemy)target;
        if (oldTarget is null)
        {
            oldTarget = targetOfThisStatus.target;
        }

        int randomIndex = UnityEngine.Random.Range(0, args.Length);
        targetOfThisStatus.target = (Entity)args[randomIndex];
    }
    public override void EndOfStatus(Entity target, params object[] args)
    {
        Enemy targetOfThisStatus = (Enemy)target;
        targetOfThisStatus.target = oldTarget;
    }
}

public class Block : Ability
{
    public Block(int value, int cost): base(value, cost)
    {
        name = Actions.Block;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.block += value;
    }
}

public class Heal : Ability
{
    public Heal(int value, int cost): base(value, cost)
    {
        name = Actions.Heal;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.RestoreHealth(value);
    }
}
public class FullHeal : Ability
{
    public FullHeal(int value, int cost): base(value, cost)
    {
        name = Actions.FullHeal;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.RestoreHealth(target.MaxHealth);
    }
}
public class IncreaseDamage : Ability
{
    public IncreaseDamage(int value, int cost): base(value, cost)
    {
        name = Actions.IncreaseDamage;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Ability ab in target.abilityList)
        {
            ab.add += value;
        }
    }
}
public class IncreaseAttacksDamage : Ability
{
    public IncreaseAttacksDamage(int value, int cost): base(value, cost)
    {
        name = Actions.IncreaseDamage;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Ability ab in target.abilityList)
        {
            if (ab.tags.Contains(Tags.Attack))
                ab.add += value;
        }
    }
}

public class Flight : Ability
{
    public Flight(int value, int cost): base(value, cost)
    {
        name = Actions.DoNothing;
        tags.Add(Tags.Fly); 
    }
    protected override void DoStuff(Entity target, object[] args)
    {
    }
}




/*public class BatEnemy : Enemy
{
    private int minDmg = 3;
    private int maxDmg = 5;
    private static int Health = 10;
    private int currDamage = 0;

    public BatEnemy(Entity player) : base(Health)
    {
        abilityList.Add(new AttackAbility(minDmg, 0));
        abilityList[0].targets.Add(player);
    }

    public override void TakeTurn()
    {
        currDamage = UnityEngine.Random.Range(0, maxDmg-minDmg);
        abilityList[0].add = currDamage;
        abilityList [0].ExecAbility();
    }

    
}*/