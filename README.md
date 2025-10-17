# Clashing Armies

A Unity-based strategy and combat simulation prototype designed with a focus on clean architecture.  
This document outlines the core thinking process, architectural principles, and development plan.

---

## 🧠 Thinking Process

**Flowchart**
https://miro.com/app/board/uXjVJ4qPYqI=/?share_link_id=42569797821

**Design Approach**
- Build systems independently and connect them through interfaces.
- Establish clear modular systems for units, health, and combat.
- Ensure scalability through patterns like *Pooling*, *Observer* and *State Machines*

## 🧱 Patterns

- **Pooling System:** Efficient reuse of objects.
- **Observer Pattern:** Event-driven communication between systems.
- **State Machine (FSM):** Unit behavior control (patrol, random move, combat, death, victory).
- **MVP / MVC:** Separation of logic and presentation.
