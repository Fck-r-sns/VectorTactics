using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public static class AddressProvider
    {
        private static int nextAddress = Defines.FIRST_FREE_ADDRESS;

        public static int GetFreeAddress()
        {
            return nextAddress++;
        }
    }
}