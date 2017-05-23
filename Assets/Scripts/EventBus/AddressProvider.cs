using UnityEngine;

namespace EventBus
{
    public class AddressProvider : MonoBehaviour
    {
        private int nextAddress = Defines.FIRST_FREE_ADDRESS;

        public int GetFreeAddress()
        {
            return nextAddress++;
        }
    }
}