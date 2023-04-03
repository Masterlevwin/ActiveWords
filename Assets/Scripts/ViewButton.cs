using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewButton : MonoBehaviour
{
  private Button btn;
  private TMP_Text txt;
  
  [SerializeField]
  private int price; 
  
  private void OnEnable()
  {
    btn = GetComponent<Button>();
    txt = GetComponentInChildren<TMP_Text>();
  }
  
  private void Upgrade( byte ability )
  {
    if( ability == 1 ) GameBase.G.pl.SetDamage( 1 );
    if( ability == 2 ) GameBase.G.pl.SetHit( ++pl.maxHit );
    if( ability == 3 ) GameBase.G.player.maxSpeed++;
    if( ability == 4 ) GameBase.G.pl.SetSpeed( 2 );
    GameBase.G.coins_count -= price;
    price *= 2;
  }
  
  private void Update()
  {
    btn.interactable = ( GameBase.G.coins_count >= price );
    txt.text = ${ price };
  }
}
