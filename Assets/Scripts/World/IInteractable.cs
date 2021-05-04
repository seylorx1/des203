using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInteractable {
    public enum Type {
        Uninteractable,
        Interactable,
        Breakable
    };
    public Type GetInteractableType();
}
