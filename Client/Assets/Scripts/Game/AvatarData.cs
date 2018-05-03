using UnityEngine;

using SharedLibrary;

public class AvatarData
{
    GameObject avatar;
    Vec3 position = new Vec3();
    Vec4 rotation = new Vec4();
    string username;

    public AvatarData()
    {
        Vec3 defaultVec3 = new Vec3();
        defaultVec3.x = 0.0f;
        defaultVec3.y = 0.0f;
        defaultVec3.z = 0.0f;

        Vec4 defaultVec4 = new Vec4();
        defaultVec4.x = 0.0f;
        defaultVec4.y = 0.0f;
        defaultVec4.z = 0.0f;
        defaultVec4.w = 0.0f;

        position = defaultVec3;
        rotation = defaultVec4;
        username = Commons.defaultName;
        avatar = null;
    }

    public void SetUsername(string name)
    {
        username = name;
    }

    public string GetUsername()
    {
        return username;
    }

    public void SetAvatar(GameObject model)
    {
        avatar = model;
    }

    public GameObject GetAvatar()
    {
        return avatar;
    }

    public void ChangePosition(float x, float y, float z)
    {
        position.x = x;
        position.y = y;
        position.z = z;
    }

    public Vec3 GetPosition()
    {
        return position;
    }

    public void ChangeRotation(float x, float y, float z)
    {
        rotation.x = x;
        rotation.y = y;
        rotation.z = z;
    }

    public Vec4 GetRotation()
    {
        return rotation;
    }
}
