using CrowdControl.Client.Unity;
using Newtonsoft.Json.Linq;

/// <summary>
/// Represents metadata for the current number of spheres in the environment, providing a read-only value that reflects
/// the active sphere count.
/// </summary>
/// <remarks>This class raises the Updated event whenever the number of spheres changes, allowing subscribers to
/// react to changes in the sphere count. It is typically used to monitor and respond to dynamic changes in the
/// environment where spheres are created or destroyed.</remarks>
public class NextSkyboxName : UnityMetadataBase<string>
{
    /// <inheritdoc cref="IMetadata.Key"/>
    public override string Key => "nextSkyboxName";

    /// <inheritdoc cref="IMetadata{string}.Value"/>
    public override string Value => m_value;
    private string m_value;

    /// <summary>
    /// Updates the current value of the metadata and raises the Updated event to notify subscribers of the change.
    /// </summary>
    /// <param name="newValue">The new value to set for the metadata.</param>
    public void UpdateValue(string newValue)
    {
        m_value = newValue;
        OnUpdated();
    }

    /// <summary>
    /// Attempts to serialize the current object to a JToken representation.
    /// </summary>
    /// <param name="value">When this method returns, contains the serialized JToken representation of the current object.</param>
    /// <returns>true if the object was successfully serialized; otherwise, false.</returns>
    public override bool TryGetSerialized(out JToken value)
    {
        value = JToken.FromObject(m_value);
        return true;
    }
}
