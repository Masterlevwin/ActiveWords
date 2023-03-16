using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour
{
  void Start()
  {
    EventManager.OnLeaveCreated();
  }
}
