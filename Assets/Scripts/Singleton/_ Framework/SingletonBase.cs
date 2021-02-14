using UnityEngine;
using TypeReferences;

[System.Serializable]
public class SingletonBase {

    #region Singleton Type
    [SerializeField]
    private ClassTypeReference m_singletonType;
    public ClassTypeReference SingletonType {
        get {
            return m_singletonType;
        }
    }
    #endregion

    #region Data Type
    [SerializeField]
    private ClassTypeReference m_dataType;
    public ClassTypeReference DataType {
        get {
            return m_dataType;
        }
    }
    #endregion

    #region Data
    [SerializeField]
    private ScriptableObject m_data;

    [SerializeField]
    private string _rawData;
    public ScriptableObject Data {
        get {
            //If data is reset, overwrite it with the raw data available.
            if(ResetData(false)) {
                UpdateDataFromRaw();
            }

            return m_data;
        }
    }
    #endregion

    public SingletonBase(ClassTypeReference singletonType, ClassTypeReference dataType) {
        m_singletonType = singletonType;
        m_dataType = dataType;


        ResetData(false);
    }

    /// <summary>
    /// Sets internal raw data infromation from the data object.
    /// </summary>
    public void UpdateRawFromData() {
        _rawData = JsonUtility.ToJson(m_data);
    }

    /// <summary>
    /// Sets the data object based on internal raw data information.
    /// </summary>
    public void UpdateDataFromRaw() {
        JsonUtility.FromJsonOverwrite(_rawData, m_data);
    }


    /// <summary>
    /// Creates a new internal data instance of type DataType.
    /// </summary>
    /// <param name="force">Ignore checking to see if internal data is null.</param>
    /// <returns>New data instance created success.</returns>
    public bool ResetData(bool force) {
        if (m_data == null || force) {
            m_data = ScriptableObject.CreateInstance(m_dataType.Type);
            return true;
        }
        return false;
    }
}