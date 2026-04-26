using UnityEngine;

public class RotationTransformation : Transformation {
    [SerializeField]
    Vector3 rotation;

    private Vector3 rotateZ(Vector3 point) {
        /* Demonstration function for rotating around the Z axis.
        
         * Rotating a point aroud an axis is like spinning a wheel.
         * Because Unity uses left-handed coordinates (looking down on +z),
         * positive rotations make the "wheel" move counterclockwise when looking
         * down through the +z diretion.


         * If we consider a circle with unit radius (r=1),
         * and assign points alignins with the x and y axes: p1 = x(1, 0) and p2 = y(0, 1),
         * rotation in 90 degree steps will result in points which coordinates are always 0, 1 or -1.
         * p1 becomes (0,1) after the first rotation, while p2 becomes (-1, 0).
         * after the second rotation. p1 becomes (-1, 0) while p2 is (0, -1)
         * the third rotation has p1 (0, -1) and p2 (1,0), and the fourth rotation resets our points.

         * If the rotation is done in 45 degree steps, the points will align with the diagonals of the XY plane.
         * By decreasing the step size, what results is a sine wave. The Sine wave matches the y coordinate
         * when starting at (1,0), and the Cosine wave matches the x coordinate.

        */
        float radZ = rotation.z * Mathf.Deg2Rad;
        float sin_z = Mathf.Sin(radZ);  // compute the sine and cosine values of the desired rotation.
        float cos_z = Mathf.Cos(radZ);

        /* 2D points (x,y) can be decomposed as xX + yY. Without any rotations, this is equivalent to
         * x(1,0) + y(0,1), which indeed results in (x,y). When rotating, this decomposition can be replaced with
         * x(cos Z, sin Z) + y(-sin Z, cos Z). This is analogous to scaling a point so it falls in the unit circle,
         * applying the rotation and then scaling it back. Compressing into a single coordinate pair results in 
         * (x cos Z - y sin Z, x sin Z + y cos Z)
        */
        return new Vector3(
            point.x * cos_z - point.y * sin_z,
            point.x * sin_z + point.y * cos_z,
            point.z
        );
    }

    public override Vector3 Apply(Vector3 point) {
        /* To rotate a point around multiple axes simultaneously,
         * a transformation matrix needs to be employed.
         * The concept is similar to the previous rotation around the z axis.
         
         * x(cos Z, sin Z) + y(-sin Z, cos Z) decomposition is really a 2x1 matrix multiplication:
         *  | cos Z  -sin Z | | x |
         *  | sin Z   cos Z | | y | 

         * To work in a 3D coordinate system, the z coordinate needs to remain unaltered.
         * This can be achieved  with the following matrix:
         *  | cos Z  -sin Z  0 | | x |
         *  | sin Z   cos Z  0 | | y | 
         *  |  0       0     1 | | z | 

         * which results in | x cos Z - y sin Z |
         *                  | x sin z + Y cos z |
         *                  |         z         |

         * extending this logic to the x and y axes:
         * the x axis start as [1, 0, 0] and ends up as [0, 0, -1] after a 90 degree rotation.
         * This can be represented by a |  cos Y |
         *                              |    0   |
         *                              | -sin Y |
         * The z axis lags 90 degrees behind which can be translated to  | sin Y |
         *                                                               |   0   |
         *                                                               | cos Y |
         * By keeping the Y axis constant, the resulting matrix is:
         *  |  cos Y   0   sin Y |
         *  |    0     1     0   | 
         *  | -sin Y   0   cos Y | 

         * The third rotation matrix keeps X constant:

         * | 1     0       0   |
         * | 0   cos X  -sin X |
         * | 0   sin X   cos X |

         * Each of these matrices represent a rotation along a single axis.
         * A generalized matrix can be created by combining the 3:
         * first rotating around the Z axis, then around Y and finally around X (Unity's rotation is apllied as ZXY)

         * The Y * Z matrix is as follows:
        
         * |  cos_y cos Z   -cos Y sin Z  sin Y |
         * |     sin Z        cos Z        0   |
         * | -sin Y cos Z   sin Y sin Z  cos Y |


         * And finally X * (Y * Z):
         * |           cos Y cos Z                     -cos Y sin Z                    sin Y    |
         * | cos X sin Z + sin X sin Y cos Z    cos Xcos Z - sin X sin Y sin Z      -sin X cos_y |
         * | sin X sin Z - cos X sin Y cos Z    sin X cos Z + cos X sin Y sin Z      cos X cos_y |
        */

        float x_rad = rotation.x * Mathf.Deg2Rad;
        float y_rad = rotation.y * Mathf.Deg2Rad;
        float z_rad = rotation.z * Mathf.Deg2Rad;

        float sin_x = Mathf.Sin(x_rad);
        float sin_y = Mathf.Sin(y_rad);
        float sin_z = Mathf.Sin(z_rad);

        float cos_x = Mathf.Cos(x_rad);
        float cos_y = Mathf.Cos(y_rad);
        float cos_z = Mathf.Cos(z_rad);

        Vector3 x_axis = new Vector3(
            cos_y * cos_z,
            cos_x * sin_z + sin_x * sin_y * cos_z,
            sin_x * sin_z - cos_x * sin_y * cos_z
        );

        Vector3 y_axis = new Vector3(
          -cos_y * sin_z,
          cos_x * cos_z - sin_x * sin_y * sin_z,
          sin_x * cos_z + cos_x * sin_y * sin_z
        );

        Vector3 z_axis = new Vector3(
            sin_y,
            -sin_x * cos_y,
            cos_x * cos_y
        );

        return x_axis * point.x + y_axis * point.y + z_axis * point.z;
    }
}