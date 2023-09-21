using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace CE6127.Tanks.AI
{
    /// <summary>
    /// Class <c>ChaseState</c> represents the state of the tank when it is Chasing the player.
    /// </summary>
    internal class ChaseState : BaseState
    {
        private TankSM m_TankSM;        // Reference to the tank state machine.
        private Vector3 m_Destination;  // Destination for the tank to move to.

        /// <summary>
        /// Constructor <c>ChaseState</c> constructor.
        /// </summary>
        public ChaseState(TankSM tankStateMachine) : base("Chasing", tankStateMachine) => m_TankSM = (TankSM)m_StateMachine;

        /// <summary>
        /// Method <c>Enter</c> on enter.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            m_TankSM.SetStopDistanceToZero();

            m_TankSM.StartCoroutine(Chasing());
        }

        /// <summary>
        /// Method <c>Update</c> update logic.
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (m_TankSM.Target != null)
            {
                // Chase the target.
                var dist = Vector3.Distance(m_TankSM.transform.position, m_TankSM.Target.position);
                // Chase the target until the distance between the tank and the target is less than the stopping range.
                if (dist > m_TankSM.TargetDistance || (Physics.Raycast(m_TankSM.transform.position, m_TankSM.transform.forward, out RaycastHit Hit, dist) && Hit.transform != m_TankSM.Target))
                {
                    // Calculate the distance between the tank and the target.
                    // var lookPos = m_TankSM.Target.position - m_TankSM.transform.position;
                    // lookPos.y = 0f;
                    // var rot = Quaternion.LookRotation(lookPos);
                    // m_TankSM.transform.rotation = Quaternion.Slerp(m_TankSM.transform.rotation, rot, m_TankSM.OrientSlerpScalar);
                    m_TankSM.NavMeshAgent.SetDestination(m_TankSM.Target.position);
                }
                else if (Physics.Raycast(m_TankSM.transform.position, m_TankSM.transform.forward, out RaycastHit hit, dist))
                {
                    // If there is an obstacle, check if the obstacle is the target.
                    if (hit.transform == m_TankSM.Target)
                    {
                        m_TankSM.NavMeshAgent.SetDestination(m_TankSM.transform.position);
                        m_TankSM.ChangeState(m_TankSM.m_States.FireOnce);
                    }
                }
                else{
                    m_TankSM.NavMeshAgent.SetDestination(m_TankSM.Target.position);
                }
                
                
            }
        }

        /// <summary>
        /// Method <c>Exit</c> on exiting ChaseState.
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            m_TankSM.StopCoroutine(Chasing());
        }

        /// <summary>
        /// Coroutine <c>Chasing</c> Chasing coroutine.
        /// </summary>
        IEnumerator Chasing()
        {
            while (true)
            {
                // Calculate a random destination within the patrol range.
                var destination = Random.insideUnitCircle * Random.Range(m_TankSM.PatrolMaxDist.x, m_TankSM.PatrolMaxDist.y);
                m_Destination = m_TankSM.transform.position + new Vector3(destination.x, 0f, destination.y);

                float waitInSec = Random.Range(m_TankSM.PatrolWaitTime.x, m_TankSM.PatrolWaitTime.y);
                yield return new WaitForSeconds(waitInSec);
            }
        }
    }
}
