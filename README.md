# VoxelForge
Describes voxel creation through object collision
A voxel-based 3D mesh cutting system using **Tool–Job collision detection**.

## Overview

The project simulates a cutting operation where a **Tool** interacts with a **Job** containing a 3D mesh.

The mesh is represented using voxels to detect the collision region. Based on the collision, the original mesh is **cut and divided** into separate parts.

```text
Tool
  ↓
Collision Detection
  ↓
Job / Mesh
  ↓
Voxel Collision
  ↓
Cut Region
  ↓
Mesh Division
```

## Main Components

- **Tool** — Represents the cutting geometry.
- **Job** — Represents the object being processed.
- **Mesh** — The geometry to be cut.
- **Voxel** — Provides a spatial representation of the mesh.
- **Collision** — Detects the intersection between the Tool and Job.
- **Mesh Cutter** — Divides the mesh based on the collision.

## Workflow

1. Create a Tool.
2. Create a Job with a mesh.
3. Generate the voxel representation.
4. Detect Tool–Job collisions.
5. Identify the affected voxels/mesh region.
6. Cut and divide the mesh.
7. Update the Job with the resulting geometry.

## Goal

The goal of this project is to efficiently perform **collision-based 3D mesh cutting** using voxel-based spatial processing.
