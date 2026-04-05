using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Core.InputManagement
{
    public class HushHushSecretClassNoPeeking : MonoBehaviour
    {
        [SerializeField] private ScriptableEventNoParam healEvent;

        private enum Input
        {
            None,
            Up,
            Down,
            Left,
            Right,
            A,
            B
        }
        
        private NESActionReader _actionReader;

        private List<Input> konamiSequence;
        private int index;
        
        private void Start()
        {
           _actionReader = GetComponent<NESActionReader>();
           _actionReader.OnDPadInput += KonamiCode;
           _actionReader.OnTapInteract += A;
           _actionReader.OnTapAltInteract += B;

           konamiSequence = new()
           {
               Input.Up, Input.Up, Input.Down, Input.Down, Input.Left, Input.Right, Input.Left, Input.Right, 
               Input.B, Input.A
           };
        }

        private void OnDestroy()
        {
            _actionReader.OnDPadInput -= KonamiCode;
            _actionReader.OnTapInteract -= A;
            _actionReader.OnTapAltInteract -= B;
        }

        private void B()
        {
            if (konamiSequence[index] == Input.B)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            if (index == konamiSequence.Count)
            {
                BooYaKaSha();
            }
        }

        private void A()
        {
            if (konamiSequence[index] == Input.A)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            if (index == konamiSequence.Count)
            {
                BooYaKaSha();
            }
        }

        private void KonamiCode(Vector2 direction)
        {
            if (direction == Vector2.zero)
                return;
            
            Input input = Input.None;

            if (direction == Vector2.up)
            {
                input = Input.Up;
            }
            else if (direction == Vector2.down)
            {
                input = Input.Down;
            }
            else if (direction == Vector2.left)
            {
                input = Input.Left;
            }
            else if (direction == Vector2.right)
            {
                input = Input.Right;
            }
            else
            {
                input = Input.None;
            }

            if (input == konamiSequence[index])
            {
                index++;
            }
            else
            {
                index = 0;
            }
            
            

            if (index == konamiSequence.Count)
            {
                BooYaKaSha();
            }
        }

        private void BooYaKaSha()
        {
            EnemyBase[] findObjectsByType = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            foreach (EnemyBase enemy in findObjectsByType)
            {
                enemy.Damage(500f);
            }
                
            healEvent.Raise();
            index = 0;
        }
    }
}
