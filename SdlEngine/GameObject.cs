using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class GameObject
    {
        private static int NextId = 0;

        public int Id { get; private set; }

        // Helps with debugging
        public string Name { get; set; } = "Anonymous";

        public TransformComponent Transform { get; set; }
        public bool IsAlive { get; set; } = true;

        private List<Component> Components { get; set; } = new List<Component>();

        public GameObject()
        {
            Initialize();
        }

        private void Initialize()
        {
            Id = NextId++;
            Transform = new TransformComponent();
            AddComponent(Transform);
        }

        public void Destroy()
        {
            IsAlive = false;
        }

        public void AddComponent(Component component)
        {
            // TODO: Ensure this component doesn't eist
            component.Owner = this;
            Components.Add(component);
        }

        public T GetComponent<T>()
        {
            return Components.OfType<T>().FirstOrDefault();
        }

        public List<T> GetComponents<T>()
        {
            return Components.OfType<T>().ToList();
        }

        // Awake is called after creation to ensure all required components are present.
        public virtual void Awake()
        {
            foreach (Component component in Components)
            {
                component.Awake();
            }
        }

        // Start is called after Awake() to initialize variables.
        public virtual void Start()
        {
            foreach (Component component in Components)
            {
                component.Start();
            }
        }

        public virtual void Update()
        {
            foreach (Component component in Components)
            {
                component.Update();
            }
        }

        public virtual void LateUpdate()
        {
            foreach (Component component in Components)
            {
                component.LateUpdate();
            }
        }

        public virtual void Render()
        {
            // TODO: This feels expensive to do on every frame whe most components only have one drawable
            foreach (DrawableComponent drawableComponent in GetComponents<DrawableComponent>())
            {
                drawableComponent.Render();
            }
        }

        public override string ToString()
        {
            return $"G[ {Name}: {Transform.Position} ]";
        }
    }
}
