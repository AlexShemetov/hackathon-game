using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public AudioClip ouch;

        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public float chaseSpeed = 3.0f;
        private Transform playerTransform;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log("Enemy route to " + playerTransform);
            }
            else
            {
                Debug.LogError("Player not found! Ensure the player GameObject has the tag 'Player'.");
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (playerTransform != null)
            {
                float step = chaseSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, step);

                // Update sprite direction based on movement
                control.move.x = playerTransform.position.x - transform.position.x;
                control.move.y = playerTransform.position.y - transform.position.y;
                Debug.Log("Enemy moved to: " + control.move);
                spriteRenderer.flipX = control.move.x < 0;
            }
        }
    }
}