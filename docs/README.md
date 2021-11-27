# READ ME

Browse this directory for design and architecture.

This directory may be refactored as documentation is added, to make it
convenient to browse using web-based GIT front-ends, such as GitHub and
BitBucket.

## Construction of Plant UML Diagrams

The PlantUML diagrams are created within Visual Studio after installing the
`plantuml` extension, and configuring it to use a local server. The `plantuml`
extension then has shortcuts to create the SVG files, which are then directly
imported into the Markdown documentation.

The extra steps for generating and committing the diagrams is necessary to allow
for the widest audience when reviewing documentation. Prerendered diagrams are
preferred as:

* Proxies don't work as they pull in the diagram defined by HEAD in Git
* they solve the problem and are committed at the same time as the documentation
  itself, so there is consistency.
* they are viewed directly in the browser for GitHub and BitBucket
* they are rendered directly using Visual Studio Code, to preview Markdown.
