using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleMovements : MonoBehaviour
{
    [SerializeField] private float minTimeToRotate = 3f, maxTimeToRotate = 5f;
    private float timer;
    [SerializeField] private List<float> rotations = new();
    private Vector3 currentDirection;
    private int currentRotationIndex = 0;
    public bool active = false;
    private AIMovementComponent movement;

    public void Awake()
    {
        movement = GetComponent<AIMovementComponent>();
    }

    public void Start()
    {
        PickNewDirection();
        ResetTimer();
    }

    public void Update()
    {
        if (!active) return;

        movement.LookAtDirection(currentDirection);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            PickNewDirection();
            ResetTimer();
        }

    }

    private void PickNewDirection()
    {
        if (rotations.Count == 0)
            return;

        float yRotation = rotations[currentRotationIndex];

        Quaternion rot = Quaternion.Euler(0f, yRotation, 0f);

        currentDirection = rot * Vector3.forward;

        currentRotationIndex++;

        if (currentRotationIndex >= rotations.Count)
            currentRotationIndex = 0;
    }

    private void ResetTimer()
    {
        timer = Random.Range(minTimeToRotate, maxTimeToRotate);
    }
}
