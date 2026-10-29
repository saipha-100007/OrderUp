using UnityEngine;

public class FrenchFriesAssemblyArea : MonoBehaviour
{
    private FrenchFriesCook currentFries;

    private void OnTriggerEnter(Collider other)
    {
        FrenchFriesCook fries =
            other.GetComponentInParent<FrenchFriesCook>();

        if (fries == null)
            return;

        if (!fries.IsCooked())
            return;

        currentFries = fries;

        Debug.Log("[FRIES AREA] RIPE FRIES ENTERED");
    }

    private void OnTriggerExit(Collider other)
    {
        FrenchFriesCook fries =
            other.GetComponentInParent<FrenchFriesCook>();

        if (fries == null)
            return;

        if (fries != currentFries)
            return;

        currentFries = null;

        Debug.Log("[FRIES AREA] RIPE FRIES LEFT");
    }

    public bool HasCookedFries()
    {
        return currentFries != null &&
               currentFries.IsCooked();
    }

    public FrenchFriesCook GetFries()
    {
        if (currentFries != null &&
            currentFries.IsCooked())
        {
            return currentFries;
        }

        return null;
    }
    public void ClearFries()
    {
        currentFries = null;
    }
}