﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/**
 * Script for placing an object on a plane
 */
public class PlaceOnPlane : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject objectToPlace;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private ARRaycastManager arRaycast;
    private bool placementPoseIsValid;
    private GameObject current;

    private void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        if (arOrigin == null)
        {
            Debug.Log("Null");
        }

        arRaycast = arOrigin.GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        if (current != null)
        {
            return;
        }

        placementIndicator.SetActive(false);

        current = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        arOrigin.MakeContentAppearAt(current.transform, current.transform.position, current.transform.rotation);
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arRaycast.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (!placementPoseIsValid) return;
        placementPose = hits[0].pose;

        Debug.Log("Found pose");

        var cameraForward = Camera.current.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        placementPose.rotation = Quaternion.LookRotation(cameraBearing);
    }

    private void UpdatePlacementIndicator()
    {
        if (current != null)
        {
            return;
        }

        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}