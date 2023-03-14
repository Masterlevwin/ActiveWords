using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Enemy : Player
{
  public ovveride void SetHit(float hit)
  {
    hitPlayer = hit;
    txtHp.text = $"{hitPlayer}";
    bar.fillAmount = hitPlayer / maxHit;
    if (hitPlayer <= 0)
    {
        hitPlayer = ++maxHit;
        gameObject.SetActive(false);
        Waiter.Wait(2f, () =>
        {
            gameObject.SetActive(true);
        });
    } 
  }
}
