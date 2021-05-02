using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class VectorConverter : ITypeAdapter
{
    // To int
    public static int FloatToInt(float value) => (int)value;
    public static int Vector2ToInt(Vector2 value) => (int)value.x;
    public static int Vector2IntToInt(Vector2Int value) => (int)value.x;
    public static int Vector3ToInt(Vector3 value) => (int) value.x;
    public static int Vector3IntToInt(Vector3Int value) => (int) value.x;
    public static int Vector4ToInt(Vector4 value) => (int) value.x;
    // public static int QuaternionToInt(Quaternion value) => (int) value.x;

    // To float
    public static float intTofloat(int value) => (float)value;
    public static float Vector2Tofloat(Vector2 value) => (float)value.x;
    public static float Vector2IntTofloat(Vector2Int value) => (float)value.x;
    public static float Vector3Tofloat(Vector3 value) => (float)value.x;
    public static float Vector3IntTofloat(Vector3Int value) => (float)value.x;
    public static float Vector4Tofloat(Vector4 value) => (float)value.x;
    // public static float QuaternionTofloat(Quaternion value) => (float)value.x;
    
    // To Vector2
    public static Vector2 IntToVector2(int value) => new Vector2(value, 0);
    public static Vector2 FloatToVector2(float value) => new Vector2(value, 0);
    public static Vector2 Vector2IntToVector2(Vector2Int value) => value;
    public static Vector2 Vector3ToVector2(Vector3 value) => value;
    public static Vector2 Vector3IntToVector2(Vector3Int value) => (Vector2Int)value;
    public static Vector2 Vector4ToVector2(Vector4 value) => value;
    public static Vector2 QuaternionToVector2(Quaternion value) => new Vector2(value.x, value.y);
    
    // To Vector2Int
    public static Vector2Int intToVector2Int(int value) => new Vector2Int((int)value, 0);
    public static Vector2Int floatToVector2Int(float value) => new Vector2Int((int)value, 0);
    public static Vector2Int Vector2ToVector2Int(Vector2 value) => new Vector2Int((int)value.x, (int)value.y);
    public static Vector2Int Vector3ToVector2Int(Vector3 value) => new Vector2Int((int)value.x, (int)value.y);
    public static Vector2Int Vector3IntToVector2Int(Vector3Int value) => new Vector2Int((int)value.x, (int)value.y);
    public static Vector2Int Vector4ToVector2Int(Vector4 value) => new Vector2Int((int)value.x, (int)value.y);
    public static Vector2Int QuaternionToVector2Int(Quaternion value) => new Vector2Int((int)value.x, (int)value.y);

    // To Vector3
    public static Vector3 intToVector3(int value) => new Vector3(value, 0, 0);
    public static Vector3 floatToVector3(float value) => new Vector3(value, 0, 0);
    public static Vector3 Vector2ToVector3(Vector2 value) => (Vector3)value;
    public static Vector3 Vector2IntToVector3(Vector2Int value) => new Vector3(value.x, value.y);
    public static Vector3 Vector3IntToVector3(Vector3Int value) => (Vector3)value;
    public static Vector3 Vector4ToVector3(Vector4 value) => (Vector3)value;
    public static Vector3 QuaternionToVector3(Quaternion value) => value.eulerAngles;

    // To Vector3Int
    public static Vector3Int intToVector3Int(int value) => new Vector3Int((int)value, 0, 0);
    public static Vector3Int floatToVector3Int(float value) => new Vector3Int((int)value, 0, 0);
    public static Vector3Int Vector2ToVector3Int(Vector2 value) => new Vector3Int((int)value.x, (int)value.y, 0);
    public static Vector3Int Vector2IntToVector3Int(Vector2Int value) => (Vector3Int)value;
    public static Vector3Int Vector3ToVector3Int(Vector3 value) => new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    public static Vector3Int Vector4ToVector3Int(Vector4 value) => new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    public static Vector3Int QuaternionToVector3Int(Quaternion value) => Vector3ToVector3Int(QuaternionToVector3(value));

    // To Vector4
    public static Vector4 intToVector4(int value) => new Vector4(value, 0, 0, 0);
    public static Vector4 floatToVector4(float value) => new Vector4(value, 0, 0, 0);
    public static Vector4 Vector2ToVector4(Vector2 value) => (Vector4)value;
    public static Vector4 Vector2IntToVector4(Vector2Int value) => new Vector4(value.x, value.y);
    public static Vector4 Vector3ToVector4(Vector3 value) => (Vector4)value;
    public static Vector4 Vector3IntToVector4(Vector3Int value) => new Vector4(value.x, value.y, value.z);
    public static Vector4 QuaternionToVector4(Quaternion value) => new Vector4(value.x, value.y, value.z, value.w);

    // To Quaternion
    // public static Quaternion intToQuaternion(int value) => (Quaternion)value;
    // public static Quaternion floatToQuaternion(float value) => (Quaternion)value;
    public static Quaternion Vector2ToQuaternion(Vector2 value) => Vector3ToQuaternion(Vector2ToVector3(value));
    public static Quaternion Vector2IntToQuaternion(Vector2Int value) => Vector3ToQuaternion(Vector2IntToVector3(value));
    public static Quaternion Vector3ToQuaternion(Vector3 value) => Quaternion.Euler(value);
    public static Quaternion Vector3IntToQuaternion(Vector3Int value) => Vector3ToQuaternion(Vector3IntToVector3(value));
    public static Quaternion Vector4ToQuaternion(Vector4 value) => new Quaternion(value.x, value.y, value.z, value.w);
    
    // UVector
    public static UVector intToUVector(int value) => (UVector)value;
    public static UVector floatToUVector(float value) => (UVector)value;
    public static UVector Vector2ToUVector(Vector2 value) => (UVector)value;
    public static UVector Vector2IntToUVector(Vector2Int value) => (UVector)Vector2IntToVector2(value);
    public static UVector Vector3ToUVector(Vector3 value) => (UVector)value;
    public static UVector Vector3IntToUVector(Vector3Int value) => (UVector)Vector3IntToVector3(value);
    public static UVector Vector4ToUVector(Vector4 value) => (UVector)value;
    public static UVector QuaternionToUVector(Quaternion value) => (UVector)value;
    public static int UVectorToint(UVector value) => (int)value;
    public static float UVectorTofloat(UVector value) => (float)value;
    public static Vector2 UVectorToVector2(UVector value) => (Vector2)value;
    public static Vector2Int UVectorToVector2Int(UVector value) => Vector2ToVector2Int(value);
    public static Vector3 UVectorToVector3(UVector value) => (Vector3)value;
    public static Vector3Int UVectorToVector3Int(UVector value) => Vector3ToVector3Int(value);
    public static Vector4 UVectorToVector4(UVector value) => (Vector4)value;
    public static Quaternion UVectorToQuaternion(UVector value) => (Quaternion)value;


}
