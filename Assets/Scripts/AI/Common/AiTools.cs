using UnityEngine;

namespace Ai
{
    [RequireComponent(typeof(TerrainReasoning))]
    [RequireComponent(typeof(Navigation))]
    [RequireComponent(typeof(Shooting))]
    [RequireComponent(typeof(SoldierController))]
    public class AiTools : MonoBehaviour
    {
        public WorldState world { get { return _world; } }
        public Navigation navigation { get { return _navigation; } }
        public Shooting shooting { get { return _shooting; } }
        public TerrainReasoning terrain { get { return _terrain; } }
        public SoldierController controller { get { return _controller; } }
        public CharacterState agentState { get { return _agentState; } }
        public CharacterState enemyState { get { return _enemyState; } }

        private bool _isInited = false;
        private WorldState _world;
        private Navigation _navigation;
        private Shooting _shooting;
        private TerrainReasoning _terrain;
        private SoldierController _controller;
        private CharacterState _agentState;
        private CharacterState _enemyState;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            if (_isInited)
            {
                return;
            }

            _world = WorldState.GetInstance();
            _navigation = GetComponent<Navigation>();
            _shooting = GetComponent<Shooting>();
            _terrain = GetComponent<TerrainReasoning>();

            _controller = GetComponent<SoldierController>();
            _agentState = _controller.GetState();
            _enemyState = _agentState.enemyState;
        }
    }

}