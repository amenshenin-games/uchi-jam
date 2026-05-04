

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
    Heal,
    FullHeal,
    EnhanceAbility,
    IncreaseAttacksDamage,
    //Trap,
    Flight,
    Bold,
    Venomous,
    Fast,
    Treasure,
    Health,
    Venom,
    //Parry,
    //Stunning,
    DoubleBite,
    Status,
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
    Energy,
    StartOfTurn,
    StartOfBattle,
    Positive,
    Attack
}

public class ActionsUtility
{
    public static Ability GetActionFromString(string actionStr, int strength, int cost)
    {
        Actions action;
        Enum.TryParse(actionStr, out action);

        Ability a;
        switch (action)
        {
            case Actions.Attack:
                a = new AttackAbility(strength, cost);
                break;
            case Actions.Poison:
                a = new StatusAbility<Poisoned>(strength, cost, 2, new List<Tags> { Tags.Venom, Tags.Status, Tags.Attack });
                break;
            case Actions.SuperBite:
                a = new SuperBiteAbility(strength, cost);
                break;
            case Actions.Confusion:
                a = new Confuse(strength, cost, 2);
                break;
            case Actions.Block:
                a = new Block(strength, cost);
                break;
            case Actions.Heal:
                a = new Heal(strength, cost);
                break;
            case Actions.FullHeal:
                a = new FullHeal(strength, cost);
                break;
            case Actions.EnhanceAbility:
                a = new EnhanceAbility(strength, cost);
                break;
            case Actions.IncreaseAttacksDamage:
                a = new IncreaseAttacksDamage(strength, cost);
                break;
            case Actions.Flight:
                a = new Flight(strength, cost);
                break;
            case Actions.Bold:
                a = new Bold(strength, cost);
                break;
            case Actions.Venomous:
                a = new Venomous(strength, cost);
                break;
            case Actions.Fast:
                a = new Fast(strength, cost);
                break;
            case Actions.Treasure:
                a = new Treasure(strength, cost);
                break;
            case Actions.Health:
                a = new Health(strength, cost);
                break;
            case Actions.Venom:
                a = new Venom(strength, cost);
                break;
            case Actions.DoubleBite:
                a = new DoubleBite(strength, cost);
                break;
            default: // Optional fallback
                a = new AttackAbility(strength, cost);
                break;
        }

        return a;
    }
}

[Serializable]
public class GenericEnemy : Enemy
{
    /// <summary>
    /// Класс, позволяющий сериализовать противников
    /// </summary>
    public int currAbility = 0;
    public int id; 
    public int Health; 
    public List<string> ActionStrList;
    public  List<int> Strength;
    public string Image; 
    public int ChallengeRating;
    public List<string> Icons; 

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
    public GenericEnemy(GenericEnemy other) : base(other.Health)
    {
        this.id = other.id;
        this.Health = other.Health;
        this.Image = other.Image;
        this.ChallengeRating = other.ChallengeRating;
        this.Icons = other.Icons;
        
        // Создаем новые списки, чтобы данные не были связаны
        this.ActionStrList = new List<string>(other.ActionStrList);
        this.Strength = new List<int>(other.Strength);
        
        // Инициализируем здоровье и способности
        this.Init();
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
            Debug.Log(id);
            abilityList.Add(ActionsUtility.GetActionFromString(actionInStr, Strength[i], 0));
            i++;
        }
    }

    /// <summary>
    /// Обычный противник просто выбирает следующее из списка своих действий.
    /// </summary>
    public override void TakeTurn(params object[] args)
    {
        block = 0;

        if (abilityList[currAbility].tags.Contains(Tags.Positive))
            abilityList[currAbility].ExecAbility(this, args);
        else
            abilityList[currAbility].ExecAbility(target, args); 

        currAbility++;
        if (currAbility >= abilityList.Count)
        {
            currAbility = 0;
        }
        Debug.Log(target.CurrentHealth);
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
public class DoubleBite : AttackAbility
{
    public DoubleBite(int value, int cost): base(value, cost)
    {
        name = Actions.DoubleBite;
        repeats = 2;
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
        tags.Add(Tags.Venom);
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
    public StatusAbility(int value, int cost, int duration, List<Tags> additionalTags): base(value, cost)
    {
        tags = additionalTags;
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
        int rand = UnityEngine.Random.Range(1, 101);
        Debug.Log(value);
        Debug.Log(rand);
        if (value >= rand)
        {   
            Debug.Log(target.MaxHealth / 2);
            target.TakeDamage((int)(target.MaxHealth / 2));
        }
    }
}
public class Confuse: Ability
{
    int duration;
    public Confuse(int value, int cost, int duration): base(value, cost)
    {
        tags.Add(Tags.Active);
        tags.Add(Tags.Status);
        this.duration = duration;
    }
    protected override void DoStuff(Entity target, params object[] args)
    {
        Confused pStatus = new Confused();
        pStatus.Init(value, 0, 2);
        Debug.Log(args.Length);
        for (int i=0; i<args.Length; i++)
        {
            GenericEnemy enemy = (GenericEnemy)args[i];
            Debug.Log(enemy.id);
            if (!enemy.statusEffects.ContainsKey(pStatus.statusName))
            {
                enemy.statusEffects.Add(pStatus.statusName, pStatus);
            }
            else
            {
                enemy.statusEffects[pStatus.statusName].duration += 1;
            }
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
        tags.Add(Tags.Positive);
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
        tags.Add(Tags.Positive);
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
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.RestoreHealth(target.MaxHealth);
    }
}
public class EnhanceAbility : Ability
{
    public EnhanceAbility(int value, int cost): base(value, cost)
    {
        name = Actions.EnhanceAbility;
        tags.Add(Tags.Active);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
        tags.Add(Tags.Positive);
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
        name = Actions.IncreaseAttacksDamage;
        tags.Add(Tags.Passive);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Health);
        tags.Add(Tags.StartOfTurn);
        tags.Add(Tags.Positive);
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
        tags.Add(Tags.Passive);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
    }
}
public class Bold : Ability
{
    public Bold(int value, int cost): base(value, cost)
    {
        name = Actions.Bold;
        tags.Add(Tags.Passive);
        tags.Add(Tags.Energy);
        tags.Add(Tags.StartOfBattle);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        target.abilityList[0].cost += value;
        target.abilityList[0].repeats += value;
    }
}
public class Venomous : Ability
{
    public Venomous(int value, int cost): base(value, cost)
    {
        name = Actions.Venomous;
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfTurn);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Ability ab in target.abilityList)
        {
            if (ab.tags.Contains(Tags.Venom))
                ab.add += 1;
        }
    }
}
public class Fast : Ability
{
    public Fast(int value, int cost): base(value, cost)
    {
        name = Actions.Fast;
        tags.Add(Tags.Energy);
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfBattle);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        Player p = (Player)target;
        Debug.Log(p.MaxEnergy);
        p.MaxEnergy += 1;
        Debug.Log(p.MaxEnergy);
    }
}
public class Treasure : Ability
{
    public Treasure(int value, int cost): base(value, cost)
    {
        name = Actions.Treasure;
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfBattle);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Ability ab in target.abilityList)
        {
            if (ab.tags.Contains(Tags.Status))
                ab.mult += 1;
        }
    }
}
public class Health : Ability
{
    public Health(int value, int cost): base(value, cost)
    {
        name = Actions.Health;
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfTurn);
        tags.Add(Tags.Defense);
        tags.Add(Tags.Positive);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        Debug.Log(value);
        target.block += value;
    }
}
public class Venom : Ability
{
    public Venom(int value, int cost): base(value, cost)
    {
        name = Actions.Venom;
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfTurn);
        tags.Add(Tags.Defense);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Entity trg in args)
        {
            Ability a = new StatusAbility<Poisoned>(1, 0, 1, new List<Tags>{Tags.Venom, Tags.Status});
            a.ExecAbility(target);
        }
    }
}

/*
public class Venom : Ability
{
    public Venom(int value, int cost): base(value, cost)
    {
        name = Actions.Fast;
        tags.Add(Tags.Passive);
        tags.Add(Tags.StartOfTurn);
        tags.Add(Tags.Defense);
    }
    protected override void DoStuff(Entity target, object[] args)
    {
        foreach (Entity target in args)
        {
            Ability a = new StatusAbility<Poisoned>(Strength[i], 0, 1);
            a.ExecAbility(target);
        }
    }
}*/








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