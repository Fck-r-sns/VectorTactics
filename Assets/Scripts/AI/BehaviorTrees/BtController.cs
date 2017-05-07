using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Bt
    {
        public class BtController : MonoBehaviour
        {

            private Node root;

            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                root.Run();
            }

            private void InitTree()
            {
                Environment envirinment = new Environment();
                root = new Selector(envirinment);

            }
        }
    }
}