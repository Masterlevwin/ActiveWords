using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlyDamage : MonoBehaviour
{
  private float damage;
  private TMP_Text damageText;
  
  public void SetDamage( float _damage ) {
    damage = Mathf.Abs( _damage );
    damageText.text = $"{damage}";
  }
}
