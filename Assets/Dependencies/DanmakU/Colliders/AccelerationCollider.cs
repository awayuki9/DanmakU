// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.DanmakU.Collider {

    /// <summary>
    /// A Danmaku Controller that speeds up or slows down danmaku so long as bullets are contacting it.
    /// </summary>
    [AddComponentMenu("Hourai.DanmakU/Colliders/Acceleration Collider")]
    public class AccelerationCollider : DanmakuCollider {

        [SerializeField, Show]
        private float _acceleration;

        /// <summary>
        /// Gets or sets the acceleration applied to affected bullets. Measured in units per second per second.
        /// </summary>
        /// <value>The acceleration applied to bullets, in absolute world units/second^2.</value>
        public float Acceleration {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        #region implemented abstract members of DanmakuCollider

        /// <summary>
        /// Handles a Danmaku collision. Only ever called with Danmaku that pass the filter.
        /// </summary>
        /// <param name="danmaku">the danmaku that hit the collider.</param>
        /// <param name="info">additional information about the collision</param>
        protected override void DanmakuCollision(Danmaku danmaku,
                                                 RaycastHit2D info) {
            danmaku.Speed += _acceleration * dt;
        }

        #endregion
    }

}