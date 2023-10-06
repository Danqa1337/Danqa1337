using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSideInteractionAbility : MonoBehaviour
{
    private SquareOrthogonal _squareOrthogonal;

    private void Awake()
    {
        _squareOrthogonal = GetComponentInParent<SquareOrthogonal>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null)
        {
            var collisionPosition = collision.gameObject.transform.position;
            var squarePosition = _squareOrthogonal.transform.position;
            var vector = collisionPosition - squarePosition;
            var direction = Utills.VectorToDirection(vector);
            _squareOrthogonal.RegisterSideInteraction(direction);
            //switch (direction)
            //{
            //    //case Direction.Null:
            //    //    break;
            //    //case Direction.Up:
            //    //    _squareOrthogonal.LineCaster.flickerTop.Flickering = true;
            //    //    break;
            //    //case Direction.Down:
            //    //    _squareOrthogonal.LineCaster.flickerBottom.Flickering = true;
            //    //    break;
            //    //case Direction.Left:
            //    //    _squareOrthogonal.LineCaster.flickerLeft.Flickering = true;
            //    //    break;
            //    //case Direction.Right:
            //    //    _squareOrthogonal.LineCaster.flickerRight.Flickering = true;
            //    //    break;
            //    //default:
            //    //    break;
            //}
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.GetComponent<Player>() != null)
        //{

        //    _squareOrthogonal.LineCaster.flickerTop.Flickering = false;
        //    _squareOrthogonal.LineCaster.flickerBottom.Flickering = false;
        //    _squareOrthogonal.LineCaster.flickerLeft.Flickering = false;
        //    _squareOrthogonal.LineCaster.flickerRight.Flickering = false;
            
       // }
    }
}
