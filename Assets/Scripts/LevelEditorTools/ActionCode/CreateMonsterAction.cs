using LevelEditorTools.Nodes;
using UnityEngine;

namespace LevelEditorTools.Action
{
    public class CreateMonsterAction : BaseAction
    {
        private float _inverval = 0;
        private Vector3 _position;

        // 这个后面会删掉，默认会使用读表 在接口内生成
        private Transform _parent;
        private GameObject _enemyPrefab;

        private float _curTime = 0;
        
        public CreateMonsterAction(int eventId, TriggerStateEnum state, float timeInterval, Vector3 pos) : base(eventId, state)
        {
            this._inverval = timeInterval;
            this._position = pos;
        }

        public void SetPrefab(Transform parent, GameObject enemy)
        {
            _parent = parent;
            _enemyPrefab = enemy;
        }

        public override void Execute()
        {
            // 生成怪物
            if (_inverval > 0)
            {
                // 间隔多久才能生成一次
                if (Time.time - _curTime > _inverval)
                {
                    CreateEnemy();
                }
            }
            else
            {
                CreateEnemy();
            }
        }

        private void CreateEnemy()
        {
            Object.Instantiate(_enemyPrefab, _position, Quaternion.identity, _parent);
            _curTime = Time.time;
        }
    }
}