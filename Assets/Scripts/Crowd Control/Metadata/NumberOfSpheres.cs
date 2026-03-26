using CrowdControl.Client.Unity;
using Newtonsoft.Json.Linq;
using System;

/// <summary>
/// Represents metadata for the current number of spheres in the environment, providing a read-only value that reflects
/// the active sphere count.
/// </summary>
/// <remarks>This class raises the Updated event whenever the number of spheres changes, allowing subscribers to
/// react to changes in the sphere count. It is typically used to monitor and respond to dynamic changes in the
/// environment where spheres are created or destroyed.</remarks>
public class NumberOfSpheres : UnityMetadataBase<int>
{
    /// <inheritdoc cref="IMetadata.Key"/>
    public override string Key => "numSpheres";

    /// <inheritdoc cref="IMetadata{int}.Value"/>
    public override int Value => SphereBehavior.InstanceCount;

    /// <summary>Cache of the last known value of the metadata to detect changes and trigger updates.</summary>
    private int m_oldValue = 0;

    /// <inheritdoc cref="IMetadata.Updated"/>
    public override event Action Updated;

    /// <summary>
    /// Attempts to serialize the current object to a JToken representation.
    /// </summary>
    /// <param name="value">When this method returns, contains the serialized JToken representation of the current object.</param>
    /// <returns>true if the object was successfully serialized; otherwise, false.</returns>
    public override bool TryGetSerialized(out JToken value)
    {
        value = JToken.FromObject(Value);
        //this particular example should always succeed
        //in a more complex implementation this might not be the case
        //so we return a bool to indicate success or failure of the serialization attempt
        return true;
    }

    /// <summary>
    /// Checks for changes in the value and raises the Updated event if the value has changed since the last update.
    /// </summary>
    /// <remarks>
    /// Checking this value every frame may be expensive or impossible depending on the complexity of the Value getter.
    /// In a more complex implementation, you might want to check for updates on a timer or based on specific events rather than every frame.
    /// </remarks>
    void Update()
    {
        int newValue = Value;
        if (newValue != m_oldValue)
        {
            m_oldValue = newValue;
            Updated?.Invoke();
        }
    }
}
