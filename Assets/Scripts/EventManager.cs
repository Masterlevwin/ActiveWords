using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
  public static event Action LeaveCreated, CoinDied;
  
  public static void OnLeaveCreated() { LeaveCreated?.Invoke(); }
  public static void OnCoinDied() { CoinDied?.Invoke(); }
}
