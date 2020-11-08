using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("Hoow far can the pickup be scattered from the dropper")]
        [SerializeField] float scatterDistance = 1;

        [SerializeField] DropLibrary dropLibrary;

        const int Attempts = 30;

        //use the unity event on the enemy prefab to trigger this public function and have the enemy drop item around him when dying
        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            //it tries 30 times and if not found a navmesh then it stops
            for (int i = 0; i < Attempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                //make sure it's a position on the navmesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            
            //in case any of this.
            return transform.position;
        }
    }
}
