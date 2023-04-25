using UnityEngine;
using TMPro;

public class FlyDamage : MonoBehaviour
{
    private TMP_Text damageText;

    private void Start()
    {
        damageText = GetComponent<TMP_Text>();
    }
    public void SetDamage( float _damage ) 
    {
        if (damageText != null)
        {
            damageText.text = $" {_damage} ";
        }
        else Debug.Log( _damage );
    }
}
