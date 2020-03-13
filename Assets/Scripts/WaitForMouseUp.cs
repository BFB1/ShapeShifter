using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForMouseUp : CustomYieldInstruction
{
    public override bool keepWaiting => !Input.GetKeyUp(KeyCode.Mouse0);
}
