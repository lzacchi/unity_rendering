using UnityEngine;
using System.Collections.Generic;

public class TransformationGrid : MonoBehaviour {
    /* Manual implementation of transformation matrices
     * To project pixels on the screen.
     * Moving, rotating and scaling a mesh by manipulating
     * vertices positions
    */

    // Creating a 3D grid of points to visualize the transformations in space
    // The grid could also be created with a particle systems. This time, it's done via prefabs
    [SerializeField]
    Transform prefab;

    [SerializeField, Range(1, 256)]
    int gridResolution = 10;

    Transform[] grid;
    List<Transformation> transformations;

    Vector3 GetCoordinates(int x, int y, int z) {
        /* The grid is cube shaped, and coordinates are based on
         * the cube's centre, with edges spanning from [-1, 1]
         * on all 3 dimensions
        */
        return new Vector3(
            x - (gridResolution - 1) * 0.5f,
            y - (gridResolution - 1) * 0.5f,
            z - (gridResolution - 1) * 0.5f
        );
    }

    Transform CreateGridPoint(int x, int y, int z) {
        /* Creating a point is done by instantiating a prefab object,
         * determining its coordiantes and associating a colour
        */
        Transform point = Instantiate<Transform>(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution
        );
        return point;
    }

    Vector3 TransformPoint(int x, int y, int z) {
        Vector3 coordinates = GetCoordinates(x, y, z);  // We get the initial vertex coordinates to avoid accumulating transformations every frame.
        for (int i = 0; i < transformations.Count; ++i) {
            coordinates = transformations[i].Apply(coordinates);
        }
        return coordinates;
    }

    void Awake() {
        grid = new Transform[gridResolution * gridResolution * gridResolution];
        for (int i = 0, z = 0; z < gridResolution; z++) {
            for (int y = 0; y < gridResolution; y++) {
                for (int x = 0; x < gridResolution; x++, i++) {
                    grid[i] = CreateGridPoint(x, y, z);
                }
            }
        }
        transformations = new List<Transformation>();
    }

    // Update is called once per frame
    void Update() {
        /* Applying the transformations in the Update method
         * makes it possible to instantly see changes in the grid
         * while in Play Mode
        */
        GetComponents<Transformation>(transformations);
        for (int i = 0, z = 0; z < gridResolution; z++) {
            for (int y = 0; y < gridResolution; y++) {
                for (int x = 0; x < gridResolution; x++, i++) {
                    grid[i].localPosition = TransformPoint(x, y, z);
                }
            }
        }
    }
}
