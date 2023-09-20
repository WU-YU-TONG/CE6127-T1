using UnityEngine;

using Debug = UnityEngine.Debug;

namespace CE6127.Tanks.AI
{
    /// <summary>
    /// Class <c>IdleState</c> represents the state of the tank when it is idle.
    /// </summary>
    internal class FireOnceState : BaseState
    {
        private TankSM m_TankSM; // Reference to the tank state machine.
        private TankShooting Shoot;
        /// <summary>
        /// Constructor <c>IdleState</c> is the constructor of the class.
        /// </summary>
        public FireOnceState(TankSM tankStateMachine) : base("FireOnce", tankStateMachine) => m_TankSM = (TankSM)m_StateMachine;

        /// <summary>
        /// Method <c>Enter</c> is called when the state is entered.
        /// </summary>
        public override void Enter() => base.Enter();

        // Declare a variable to record current time.
        private float m_CurrentTime = 0f;

        // Declare a variable to calculate launch force.
        private float m_LaunchForce = 6.5f;

        /// <summary>
        /// Method <c>Update</c> is called each frame.
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (m_TankSM.Target != null)
            {
                // Calculate the distance between the tank and the target.
                var dist = Vector3.Distance(m_TankSM.transform.position, m_TankSM.Target.position);
                var lookPos = m_TankSM.Target.position - m_TankSM.transform.position;
                lookPos.y = 0f;
                var rot = Quaternion.LookRotation(lookPos);
                m_TankSM.transform.rotation = Quaternion.Slerp(m_TankSM.transform.rotation, rot, m_TankSM.OrientSlerpScalar);

                // Add interval of 0.7f between each firing.
                m_CurrentTime += Time.deltaTime;
                if (m_CurrentTime < 0.7f)
                {
                    return;
                }
                m_CurrentTime = 0f;

                // Calculate launch force by launchforce = 1f * dist. 
                // Restrict launch force to the range of 6.5f to 30f. 
                m_LaunchForce = Mathf.Clamp(1f * dist, m_TankSM.LaunchForceMinMax.x, m_TankSM.LaunchForceMinMax.y);

                // Make the tank to fire a projectile.
                m_TankSM.LaunchProjectile(m_LaunchForce);

                if (dist > m_TankSM.StopAtTargetDist.x)
                {
                    m_TankSM.ChangeState(m_TankSM.m_States.Idle);
                }
            }
        }
    }
}
