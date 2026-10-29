using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private CustomerOrder.MenuType menu;

    public CustomerOrder.MenuType Menu => menu;
}