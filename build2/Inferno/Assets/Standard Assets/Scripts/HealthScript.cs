using UnityEngine;
using System.Collections;

// TODO: Maybe make virtual methods with bodies?
public abstract class HealthScript : MonoBehaviour {
	public abstract void HurtHealth(int amount);
	public abstract void HealHealth(int amount);
	public abstract void SetHealth(int amount);
	protected abstract void HandleDeath();
}
