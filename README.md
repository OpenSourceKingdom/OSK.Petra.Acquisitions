# OSK.Petra.Acquisitions

Provides a system that is capable of handling a variety of acquisition scenarios and providing the means to display that data to a user within an application UI. At it's core, an acquisition is the process by which a thing is acquired. This can be
a simple one time, immediate, purchase of an item to handling a queue system in which item purchases may cause a backlog of items that must be processed over time by the system. The acquisition system provides these core concepts:

- `IAcquisitionHub`: The focal point of an acquisition process. The hub represents the location where a collection of acquisitions may be retrieved and interacted with.
- `IAcquirer`: The initiator of an acquisition process. An acquirer interacts with a hub in order to determine what will be acquired. The acquirer can represent either a collective whole (e.g. a player) or an individual (e.g. player character).
- `IAcquisitionDefinition`: A data structure that describes various aspects of an acquisition a user can make, including the acquierable itself, a configuration for the acquisition process, and acquisition path information
- `IAcquisition`: A generic acquisition process that can be initiated. This handles the data and related information for a particular acquireable that can be acquired
- `ChoiceAcquisition`/`PathAcquisition`: A specific acquisition data type. An AcquisitionPath is a collection of choices that are grouped behind a root choice. This can be either a single or multi path, where a single path is a linear progression with no branching in the path and a multi path is a tree like structure that branches into many sub paths.
- `IAcquisitionHold`: A hold is a data structure that prevents an acquisition process from continuing at various parts of the acquisition process. If a hold exists, the acquisition process will wait until it completes
- `IAcquisitionRequirement`: A requirement is a limiting restriction that is placed on an acquisition definition to help determine if a given acquisition is available to a user. The requirement can be tied to specific acquirers. 
- `IAcquisitionView`: The view represents a unified overview of what an acquisition hub can provide a given acquirer at a specific moment in time. 
- `IAcquisitionSystem`: The central system by which all acquisitions, hubs, and related data are handled and processed. This is the main API that developers can interact with to generate views, load hubs and state, and more.

The above concepts creates the acquisition system. Applications will need to add the core system and related services to their dependency containers using the `AddAcquisitions` extension and then utilizing the `IAcquisitionSystem`, they will need 
to update the system as necessary and interact with it to both load acquisition state and getting the needed acquisition views to present on a UI. When retrieving acquisition information to display, the system attempts to provide as much flexibility
in variation of provided options to a user by using an acquirer mechanism. An acquirer can be thought of as the player themselves or as an individual unit or character that a player controls. This separation allows an acquisition hub to determine
if it will show only certain acquisition processes to a specific player or character or what types of discounts or pricing variation will be applied to the acquisition processes for that particular player or character. For example, an acquisition view
may be of a set of options in a barracks, where each option is a unit to purchase (infantry, tanks, etc.). These options may be unavailable to the player's enemy team and only available to them. Potentially, thew acquisition hub for the player may be 
utilized by their allies on their team or maybe it's not. Potentially, there are scenarios where you'd show a list of acquisition options that are cheaper for one acquirer vs. another - e.g. a character hero has an item in their inventory and another does not.
The main goal of the acquisition system is to provide these contextual insights to a cost provider, a hold provider, etc. to allow for changes and specific use cases for an acquisition process across multiple types of acquirers.

The system is designed to allow for creating varying types of acquisition holds and requirements. The library currently supports the following out of the box:

### Holds

- `Timer`: a hold that primarily uses a timespan to determine the length of a wait in an acquisition process.

### Requirements

- `Dependency`: a requirement that prevents the availability of an acquisition process if one or more dependencies are not acquired by the owner of the acquirer. 