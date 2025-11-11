namespace IAUS.ECS.Component
{

    [System.Serializable]
    public enum TravelPlan
    {
        none, 
        GetNewLocation,
        MoveToLocation, 
        Wait
    }
    
}