using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;


public abstract class Ability
{
    /// <summary>
    /// Класс, описывающий активные или пассивные особенности персонажей
    /// </summary>

    public Actions name; /// название
    public List<Tags> tags; /// Набор тэгов, характерезуюших способность
    public int cost = 0; /// Количество энергии, используюшееся для применения способности
    protected int initValue; /// Начальное значение эффективности
    public int add = 0; /// Модификатор, влияющий на способность, изменяется извне
    public int mult = 1; /// Мультипликатор, влияющий на способность, изменяется извне
    public int repeats = 1; /// Количество повторений способности, изменяется извне
    protected int value{ get => (int)initValue*mult + add;} /// Основной параметр, который потом следует применять в  в DoStuff()



    /// <summary>
    /// Основной конструктор
    /// </summary>
    /// <param name="tags">Набор Тэгов, согласно данным</param>
    /// <param name="initValue">Начальное значение эффективности. Например, урон для атакующих навыков или
    /// лечение.
    /// </param>
    public Ability(int initValue, int cost)
    {
        this.cost = cost;
        this.initValue = initValue;
        tags = new List<Tags>();
    }



    /// <summary>
    /// Для каждой цели применяет действие, прописанное наследником класса repeats раз.
    /// Вызывайте его для исполнения действий.
    /// </summary>
    /// <param name="args">Параметры, необходимые для выполнения действия</param>
    public virtual void ExecAbility(Entity target, params object[] args)
    {
        for (int i=0; i < repeats; i++){
            DoStuff(target, args);
        }
    }


    /// <summary>
    /// Логика действий, производимых объектом
    /// Имплементируя этот метод вы, по сути, переводите язык дизайна (пр. "нанести 2 урона") в логику игры.
    /// </summary>
    /// <param name="args">Любые параметры, необходимые для исполнения действия. Обрабатываются индивидуально при 
    /// реализации. Перый параметр всегда - цель типа Entity. Она передаётся из ExecAbility.
    /// </param>
    protected abstract void DoStuff(Entity target, params object[] args);
}


public abstract class Status : Ability
{
    /// <summary>
    /// Класс, описывающий длительные эффекты
    /// </summary>

    public int duration; /// Длительность действия эффекта. Уменьшается на 1 после вызова ExecAbility()
    public bool active = true;
    public string statusName;
    public Status(int initValue, int cost, int duration) : base(initValue, cost)
    {
        this.duration = duration;
    }

    public Status() : base(0,0)
    {
        
    }

    public void Init(int initValue, int cost, int duration)
    {
        this.initValue = initValue;
        this.cost = cost;
        this.duration = duration;
    }

    /// <summary>
    /// То же, что и в классе-родителе, но отслеживает длительность.
    /// </summary>
    /// <param name="args"></param>
    public override void ExecAbility(Entity target, params object[] args)
    {
        if (duration > 0)
        {
            for (int i=0; i < repeats*duration; i++)
                DoStuff(target, args);
                    
            duration--;
        }
        if (duration == 0)
        {
            active = false;
            EndOfStatus(target, args);
        }
    }

    public abstract void EndOfStatus(Entity target, params object[] args);
}
public class Entity
{
    /// <summary>
    /// Класс, описывающий данные и повеление персонажей: Игрока и Противников
    /// </summary>
    public float MaxHealth; /// Начальный запас здоровья
    public int CurrentHealth; /// Текущий запас здоровья
    public Dictionary<string, Status> statusEffects; /// Список Активных статусов
    public List<Ability> abilityList; ///Список доступных действий
    public int block; /// 
    
    public bool isStunned = false;
    public int dodgeChanse = 0;
    

    /// <summary>
    /// Основной конструктор
    /// </summary>
    /// <param name="Health">Максимальный запас здоровья</param>
    public Entity(float Health)
    {
        this.MaxHealth = Health;
        this.CurrentHealth = (int)Health;
        this.statusEffects = new Dictionary<string, Status> ();
        this.abilityList = new List<Ability>();
        this.block = 0;
    }

    /// <summary>
    /// Метод для получения урона. Делает проверку и вызывает поражение, если здоровье на 0
    /// </summary>
    /// <param name="damage">урон</param>
    public void TakeDamage(float damage)
    {
        CurrentHealth -= (int)damage - block;
        if (CurrentHealth <= 0)
        {
            Defeat();
        }
    }

    /// <summary>
    /// Метод для восстановления здоровья не выше максимума
    /// </summary>
    /// <param name="heal">лечение</param>
    public void RestoreHealth(float heal)
    {
        CurrentHealth += (int)heal;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = (int)MaxHealth;
        }
    }

    /// <summary>
    /// Исполнение всех статусных эффектов, обычно вызывается вначале хода
    /// </summary>
    public void StatusTick(params object[] args)
    {
        Debug.Log(statusEffects.Count);
        List<string> keys = statusEffects.Keys.ToList();
        foreach (string name in keys)
        {
            Debug.Log(name);
            if (statusEffects[name].active)
                statusEffects[name].ExecAbility(this, args);
            else
                statusEffects.Remove(name);
        }
    }

    public void Defeat()
    {
        
    }
}

public abstract class Enemy : Entity
{
    /// <summary>
    /// Класс, описывающий отличительные особенности противников
    /// </summary>
    public Entity target; ///Текушая цель противника
    public Enemy(float Health) : base(Health)
    {
    }

    /// <summary>
    /// Логика поведения конкретного противника. Выбор и исполнение действий.
    /// </summary>
    public abstract void TakeTurn(params object[] args);
}
/*
public class Player : Entity
{
    int MaxEnergy;
    int EnergyAdd;
    int CurrentEnergy;
    
    public Player(float Health): base(Health)
    {
        this.EnergyAdd = 0;
        this.MaxEnergy = 3;
        this.CurrentEnergy = MaxEnergy;
    }

    public void DoAction(int i)
    {       
        if (EnergyAdd > 0)
        {
            EnergyAdd--;
            return MaxEnergy + EnergyAdd+1;
        }
        else if (EnergyAdd < 0)
        {
            EnergyAdd--;
            return MaxEnergy + EnergyAdd+1;
        }
        
    }

}*/ 

[Serializable]
public class Item
{
    public int id;
    public string ActiveName;
    public string PassiveName;
    public string ActiveStrength;
    public string PassiveStrength;
    public string Image;
    public string Description;
    Item(int id, string ActiveName, string PassiveName, string ActiveStrength, string PassiveStrength, string Image, string Description)
    {
        this.id = id;
        this.ActiveName = ActiveName;
        this.PassiveName = PassiveName;
        this.ActiveStrength = ActiveStrength;
        this.PassiveStrength = PassiveStrength;
        this.Description = Description;
        this.Image = Image;
    }

    public override string ToString()
    {
        return $"{{ActiveString: {ActiveName}, PassiveString: {PassiveName}, ActiveStrength: {ActiveStrength}, PassiveStrength: {PassiveStrength} image: {Image}, Description: {Description}}}";
    }
}

[Serializable]
public class ItemListWrapper
{
    public List<Item> itemList;

}

