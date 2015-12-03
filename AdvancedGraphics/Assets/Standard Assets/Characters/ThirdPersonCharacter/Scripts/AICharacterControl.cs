using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        private Transform target; // target to aim for
		public Transform[] waypoints;
		private int destination;
		private bool firstMove = true;
		public bool interrupt = false;

        // Use this for initialization
        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
			agent.autoBraking = false;

			destination = 0;
			if (waypoints.Length > 0)
				target = waypoints [destination];

        }


        // Update is called once per frame
        private void Update()
        {
            if (target != null)
            {
                agent.SetDestination(target.position);
                // use the values to move the character
                character.Move(agent.desiredVelocity, false, false);
				if (agent.remainingDistance < 2f){
					if (!firstMove) {
						destination++;
						if (destination >= waypoints.Length)
							destination = 0;
						target = waypoints [destination];
						agent.SetDestination(target.position);
					} else
						firstMove = false;
				}
            }
            else
            {
                // We still need to call the character's move function, but we send zeroed input as the move param.
                character.Move(Vector3.zero, false, false);
            }
			Transform oldTarget = target;
			if (interrupt) {
				GameObject g = GameObject.Find ("Player");
				target = g.transform;
			} else
				SetTarget (oldTarget);
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }

		public void setInterrupt(bool b) {
			interrupt = b;
		}

		public bool getInterrupt() {
			return interrupt;
		}
    }
}
