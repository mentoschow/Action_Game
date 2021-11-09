using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyIUserInput : IUserInput
{
    IEnumerator Start()
    {
        while (true)
        {
            //attack = true;
            yield return 0;
        }
    }

    void Update()
    {
        UpdateDmagDvec(Dup, Dright);
        //attack = true;
    }
}
