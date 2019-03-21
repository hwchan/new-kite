using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
	
	
	
	// Stats
	public int damage = 1;
	public int health = 1;
	public float moveSpeed = 2;
	public float chargeTime = .15f;
	public float attackRange = 3.25f;
	
	// Experience
	public int attributePt = 1;	
	
	private int currentExp = 0;
	private int expToLvl = 25;
	private int level = 1;
	
	public int CurrentExp{
		get{return currentExp;}
	}
	public int ExperienceToLevel{
		get{return expToLvl;}
	}
	public int Level{
		get{return level;}
	}
	public int UpgradeSightAndRange{
		get{return upgradeSightAndRange;}
	}
	public int UpgradeMoveAndAttackSpeed{
		get{return upgradeMoveAndAttackSpeed;}
	}
	public int UpgradeAttackDamage{
		get{return upgradeAttackDamage;}
	}
	public int UpgradeHealthPoints{
		get{return upgradeHealth;}
	}	
    public float AttackRange{
        get { return attackRange; }
    }			
	
	private int upgradeSightAndRange = 1;		//Sight: increase line of sight and attack range
	private int upgradeMoveAndAttackSpeed = 1;	//Speed: increase movement and attack speed
	private int upgradeAttackDamage = 1;		//Attack: increase base attack damage
	private int upgradeHealth = 1;				//Health: increase base health pool
	
	private float baseSight;
	private float baseAttackRange;
	private float baseSpeed;
	private float baseChargeTime;
	private int baseDamage;
	private int baseHealth;
	
//	private PlayerScript ps;
	private PlayerHealthScript phs;
//	private PlayerShoot pst;
	
	void Awake(){
//		ps = VariableScript.scrPlayerScript1;
		phs = VariableScript.scrPlayerHealthScript1;
//		pst = VariableScript.scrPlayerShoot1;
		
//		baseSight = Camera.main.orthographicSize;
//		baseSpeed = ps.moveSpeed;
//		baseChargeTime = pst.TimePerCharge;
//		baseDamage = pst.damage;
//		baseHealth = phs.MAX_HP;
		baseSight = Camera.main.orthographicSize;
		baseAttackRange = attackRange;
		baseSpeed = moveSpeed;
		baseChargeTime = chargeTime;
		baseDamage = damage;
		phs.MAX_HP = health;
		baseHealth = health;
	}
	
	public void IncreaseExp(int exp){
		currentExp += exp;
		while(currentExp >= expToLvl){
			level++;
			attributePt++;
			currentExp = currentExp - expToLvl;
			expToLvl =  Mathf.RoundToInt(expToLvl * 1.75f);
			GUIManager.SetLabel(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, 20),"Level Up!",3);
		}
	}
	
	public void UpgradeSight(int i){
//		if(attributePt - i >= 0){
//			attributePt -= i;
			upgradeSightAndRange += i;
			Camera.main.GetComponent<CameraScript>().SetCameraSize( baseSight + (.2f*(UpgradeSightAndRange-1)) );
			attackRange = baseAttackRange + (.2f*(UpgradeSightAndRange-1));
//		}
	}
	
	public void UpgradeSpeed(int i){
//		if(attributePt - i >= 0){
//			attributePt -= i;
			upgradeMoveAndAttackSpeed += i;
			moveSpeed = baseSpeed + (.15f*(UpgradeMoveAndAttackSpeed-1));
			chargeTime = baseChargeTime - (.025f*(UpgradeMoveAndAttackSpeed-1));
//		}
	}
	
	public void UpgradeAttack(int i){
//		if(attributePt - i >= 0){
//			attributePt -= i;
			upgradeAttackDamage += i;
			damage = baseDamage + (1*(UpgradeAttackDamage-1));
//		}
	}
	
	public void UpgradeHealth(int i){
//		if(attributePt - i >= 0){
//			attributePt -= i;
			upgradeHealth += i;
			// TODO: update PlayerScript with new modifiers
			health = baseHealth + (1*(UpgradeHealthPoints-1));
			phs.SetMaxHealth(health);
//		}
	}
	
}
