using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewButton : MonoBehaviour
{
  private Button btn;
  private TMP_Text txt;
  
  [SerializeField]
  private int start_price;
  private int price;
  
  private void OnEnable()
  {
    btn = transform.parent.GetComponent<Button>();
    txt = GetComponentInChildren<TMP_Text>();
    price = start_price;
  }
  
  private void OnDisable()
  {
    price = start_price;
  }
  
  public void Upgrade( int ability )
  {
    SoundManager.PlaySound("UpgradeLevel");
    if ( ability == 1 ) GameBase.G.pl.SetDamage( 1 );
    if( ability == 2 ) GameBase.G.pl.SetHit( ++GameBase.G.pl.maxHit );
    if( ability == 3 ) GameBase.G.player.maxSpeed++;
    if( ability == 4 ) GameBase.G.pl.SetSpeed( 2 );
    GameBase.G.coins_count -= price;
    price *= 2;
  }
  
  private void Update()
  {
    btn.interactable = ( GameBase.G.coins_count >= price );
    txt.text = $"{ price }";
  }
}
