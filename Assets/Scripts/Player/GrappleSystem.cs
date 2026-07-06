using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Followed tutorial by Sean Duffy, who wrote a majority of the code
namespace SoulHunter.Player
{
    public class GrappleSystem : MonoBehaviour, Input.IMoveInput // (Adjusted by) Mort
    {
        public bool isColliding;

        public GameObject ropeHingeAnchor;
        public DistanceJoint2D ropeJoint;
        public Transform crosshair;
        public SpriteRenderer crosshairSprite;

        private Vector2 playerPosition;
        private PlayerAim playerAim;
        private Rigidbody2D ropeHingeAnchorRb;
        private SpriteRenderer ropeHingeAnchorSprite;

        public LayerMask ropeLayerMask;
        public LayerMask obstacleLayerMask;

        private LineRenderer ropeRenderer;
        public float maxRopeLength = 10f;
        public float climbSpeed = 3f;
        private List<Vector2> ropePositions = new List<Vector2>();

        private bool ropeAttached;
        private bool distanceSet;

        private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();

        float verticalInput;

        void Awake()
        {
            ropeJoint.enabled = false;
            playerPosition = transform.position;

            playerAim = GetComponent<PlayerAim>();
            ropeRenderer = GetComponent<LineRenderer>();

            ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
            ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            playerPosition = transform.position;

            if (PlayerBase.isPaused)
            {
                crosshairSprite.enabled = false;
                return;
            }

            if (!ropeAttached)
            {
                PlayerBase.isSwinging = false;

                SetCrosshairPosition(playerAim.aimAngle);
            }
            if (ropeAttached)
            {
                PlayerBase.isSwinging = true;
                PlayerBase.ropeHook = ropePositions.Last();

                crosshairSprite.enabled = false;

                if (ropePositions.Count > 0)
                {
                    var lastRopePoint = ropePositions.Last();
                    var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, (lastRopePoint - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint) - 0.1f, ropeLayerMask);

                    if (playerToCurrentNextHit)
                    {
                        var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                        if (colliderWithVertices != null)
                        {
                            var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                            if (wrapPointsLookup.ContainsKey(closestPointToHit))
                            {
                                ResetRope();
                                return;
                            }

                            ropePositions.Add(closestPointToHit);
                            wrapPointsLookup.Add(closestPointToHit, 0);
                            distanceSet = false;
                        }
                    }
                }
            }

            UpdateRopePositions();

            HandleRopeLength(verticalInput);

            HandleRopeUnwrap();
        }

        public void SetCrosshairPosition(float aimAngle)
        {
            if (!crosshairSprite.enabled)
            {
                crosshairSprite.enabled = true;
            }

            var x = transform.position.x + playerAim.crosshairDistance * Mathf.Cos(aimAngle);
            var y = transform.position.y + playerAim.crosshairDistance * Mathf.Sin(aimAngle);

            var crossHairPosition = new Vector3(x, y, 0);
            crosshair.transform.position = crossHairPosition;
        }

        public void ShootGrapple(Vector2 aimDirection)
        {
            if (ropeAttached) return;
            if (PlayerBase.isGrounded) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, maxRopeLength, ropeLayerMask);

            if (hit.collider != null)
            {
                ropeAttached = true;
                if (!ropePositions.Contains(hit.point))
                {
                    // Jump slightly to distance the player a little from the ground after grappling to something.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;

                    //This needs to stay as it often needs to be tested. Can be temporarily commented.
                    //Debug.Log($"Grapple is attached to {hit.transform.name}");
                }

                PlayerBase.isThrowing = true;
                AudioManager.PlaySound(AudioManager.Sound.GrappleThrow, transform.position);
            }
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        public void ResetRope()
        {
            ropeJoint.enabled = false;
            ropeAttached = false;
            PlayerBase.isSwinging = false;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, transform.position);
            ropePositions.Clear();
            ropeHingeAnchorSprite.enabled = false;

            PlayerBase.ropeHook = Vector2.zero;

            wrapPointsLookup.Clear();

            AudioManager.PlaySound(AudioManager.Sound.GrappleDetach, transform.position);
        }

        private void UpdateRopePositions()
        {
            if (!ropeAttached)
            {
                return;
            }

            ropeRenderer.positionCount = ropePositions.Count + 1;

            for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
            {
                if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
                {
                    ropeRenderer.SetPosition(i, ropePositions[i]);

                    if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                    {
                        var ropePosition = ropePositions[ropePositions.Count - 1];
                        if (ropePositions.Count == 1)
                        {
                            ropeHingeAnchorRb.transform.position = ropePosition;
                            if (!distanceSet)
                            {
                                ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                                distanceSet = true;
                            }
                        }
                        else
                        {
                            ropeHingeAnchorRb.transform.position = ropePosition;
                            if (!distanceSet)
                            {
                                ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                                distanceSet = true;
                            }
                        }
                    }
                    else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                    {
                        var ropePosition = ropePositions.Last();
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                else
                {
                    ropeRenderer.SetPosition(i, transform.position);
                }
            }
        }

        private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
        {
            var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
                position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
                position => polyCollider.transform.TransformPoint(position));

            var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
            return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
        }

        public void HandleRopeLength(float vertical)
        {
            if (vertical >= 1f && ropeAttached && !isColliding)
            {
                ropeJoint.distance -= Time.deltaTime * climbSpeed;
            }
            else if (vertical < 0f && ropeAttached)
            {
                if (ropeJoint.distance >= maxRopeLength) return;

                ropeJoint.distance += Time.deltaTime * climbSpeed;
            }
        }

        void OnTriggerStay2D(Collider2D colliderStay)
        {
            if (colliderStay.transform.gameObject.layer == obstacleLayerMask)
            {
                isColliding = true;
            }
        }

        private void OnTriggerExit2D(Collider2D colliderOnExit)
        {
            if (colliderOnExit.transform.gameObject.layer == obstacleLayerMask)
            {
                isColliding = false;
            }
        }

        void HandleRopeUnwrap()
        {
            if (ropePositions.Count <= 1)
            {
                return;
            }

            // Hinge = next point up from the player position
            // Anchor = next point up from the Hinge
            // Hinge Angle = Angle between anchor and hinge
            // Player Angle = Angle between anchor and player

            var anchorIndex = ropePositions.Count - 2;
            var hingeIndex = ropePositions.Count - 1;
            var anchorPosition = ropePositions[anchorIndex];
            var hingePosition = ropePositions[hingeIndex];
            var hingeDir = hingePosition - anchorPosition;
            var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
            var playerDir = playerPosition - anchorPosition;
            var playerAngle = Vector2.Angle(anchorPosition, playerDir);

            if (!wrapPointsLookup.ContainsKey(hingePosition))
            {
                Debug.LogError("We were not tracking hingePosition (" + hingePosition + ") in the look up dictionary.");
                return;
            }

            if (playerAngle < hingeAngle)
            {
                if (wrapPointsLookup[hingePosition] == 1)
                {
                    UnwrapRopePosition(anchorIndex, hingeIndex);
                    return;
                }

                wrapPointsLookup[hingePosition] = -1;
            }
            else
            {
                if (wrapPointsLookup[hingePosition] == -1)
                {
                    UnwrapRopePosition(anchorIndex, hingeIndex);
                    return;
                }

                wrapPointsLookup[hingePosition] = 1;
            }
        }

        void UnwrapRopePosition(int anchorIndex, int hingeIndex)
        {
            var newAnchorPosition = ropePositions[anchorIndex];
            wrapPointsLookup.Remove(ropePositions[hingeIndex]);
            ropePositions.RemoveAt(hingeIndex);

            ropeHingeAnchorRb.transform.position = newAnchorPosition;
            distanceSet = false;

            // Set new rope distance joint distance for anchor position if not yet set.
            if (distanceSet)
            {
                return;
            }
            ropeJoint.distance = Vector2.Distance(transform.position, newAnchorPosition);
            distanceSet = true;
        }

        public void HandleMoveInput(Vector2 input)
        {
            verticalInput = input.y;
        }
    }
}