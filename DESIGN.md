---
name: Fluent OCR System
colors:
  surface: '#f9f9f9'
  surface-dim: '#dadada'
  surface-bright: '#f9f9f9'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f3f3'
  surface-container: '#eeeeee'
  surface-container-high: '#e8e8e8'
  surface-container-highest: '#e2e2e2'
  on-surface: '#1a1c1c'
  on-surface-variant: '#404752'
  inverse-surface: '#2f3131'
  inverse-on-surface: '#f1f1f1'
  outline: '#717783'
  outline-variant: '#c0c7d4'
  surface-tint: '#0060ab'
  primary: '#005faa'
  on-primary: '#ffffff'
  primary-container: '#0078d4'
  on-primary-container: '#ffffff'
  inverse-primary: '#a3c9ff'
  secondary: '#5f5e5e'
  on-secondary: '#ffffff'
  secondary-container: '#e4e2e1'
  on-secondary-container: '#656464'
  tertiary: '#974700'
  on-tertiary: '#ffffff'
  tertiary-container: '#bc5b00'
  on-tertiary-container: '#ffffff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#d3e3ff'
  primary-fixed-dim: '#a3c9ff'
  on-primary-fixed: '#001c39'
  on-primary-fixed-variant: '#004883'
  secondary-fixed: '#e4e2e1'
  secondary-fixed-dim: '#c8c6c6'
  on-secondary-fixed: '#1b1c1c'
  on-secondary-fixed-variant: '#474747'
  tertiary-fixed: '#ffdbc8'
  tertiary-fixed-dim: '#ffb689'
  on-tertiary-fixed: '#311300'
  on-tertiary-fixed-variant: '#743500'
  background: '#f9f9f9'
  on-background: '#1a1c1c'
  surface-variant: '#e2e2e2'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
    letterSpacing: -0.01em
  title-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  body-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 16px
  label-caps:
    fontFamily: Inter
    fontSize: 11px
    fontWeight: '700'
    lineHeight: 16px
    letterSpacing: 0.05em
  mono-data:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 18px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  nav_rail_width: 64px
  nav_rail_expanded: 240px
  gutter: 16px
  container_padding: 24px
  stack_gap_sm: 8px
  stack_gap_md: 16px
---

## Brand & Style
The design system is engineered for a high-performance Windows 11 desktop environment. It focuses on productivity, speed, and native integration. The personality is professional, reliable, and unobtrusive, allowing users to focus on document processing tasks. 

The aesthetic adheres to **Modern Fluent Design** principles:
- **Minimalism:** Use of negative space to reduce cognitive load during complex OCR tasks.
- **Mica Material:** Application of semi-transparent, occlusion-sensing surfaces that let the desktop wallpaper bleed through slightly, creating a sense of place.
- **Hierarchy through Motion:** Subtle, purposeful transitions when navigating between the document library and processing views.

## Colors
The palette is rooted in the Windows ecosystem. The primary accent color is used sparingly for actionable elements, progress indicators, and active states. 

- **Primary (#0078D4):** Used for "Start OCR," primary buttons, and the active state of the navigation rail.
- **Backgrounds:** Use a layered approach. The main window background uses a Mica-effect light grey, while content containers (cards, document editors) use pure white to ensure maximum text contrast.
- **Typography Colors:** Primary text is nearly black for legibility, while secondary metadata (file size, date) uses a medium grey.

## Typography
Since **Segoe UI Variable** is the native system font, **Inter** is specified here as the closest highly-legible web-safe alternative that captures the Windows 11 personality.

- **Display & Titles:** Use variable weights to create clear section headers.
- **Body Text:** Optimized for long-form reading of OCR output.
- **Monospace:** **JetBrains Mono** is utilized for technical metadata, confidence scores, and raw text extraction views to provide a "developer-grade" precision feel.

## Layout & Spacing
The layout follows a **Fixed-Fluid hybrid** model typical of desktop applications.

- **Navigation Rail:** A fixed left-side bar (64px collapsed) contains primary app destinations.
- **Main Content Area:** A fluid area that uses a 12-column grid for large dashboard views, reflowing to a single column for the document reader.
- **Spacing Rhythm:** Based on an 4px/8px incremental system. Use 24px padding for main window margins and 16px for internal card padding to maintain a dense, professional information layout.

## Elevation & Depth
This design system utilizes **Tonal Layering** combined with **Ambient Shadows** to define the z-axis.

- **Layer 0 (Bottom):** Mica background, capturing window-behind colors.
- **Layer 1 (Cards/Panels):** Pure white surfaces with a 1px neutral stroke (#E5E5E5).
- **Layer 2 (Popovers/Context Menus):** Elevated surfaces featuring a 16px blur radius shadow with 8% opacity.
- **Active State:** Use a 2px bottom border on active tabs rather than heavy shadows to maintain a flat, modern look.

## Shapes
In line with Windows 11 geometry, all container corners are rounded.
- **Standard Elements:** 8px radius (Buttons, Input Fields, Checkboxes).
- **Large Containers:** 12px radius (Cards, Dialogs, Main Panels).
- **Interactive States:** On hover, list items should show a subtle 4px rounded highlight box.

## Components
- **Buttons:** Primary buttons use a solid blue background with white text. Secondary buttons use a white background with a subtle grey border.
- **Progress Bars:** Thin (4px) tracks with the Primary Blue fill. Use an indeterminate "pulse" animation for active OCR processing.
- **Navigation Rail:** Icons should be centered. Active states are indicated by a 3px vertical "pill" indicator on the far left.
- **Data Tables:** High-density rows (32px height) with a subtle #F9F9F9 hover background. Include "Confidence Score" badges using a semantic color scale (Green/Amber/Red).
- **Toggle Switches:** Fluent-style rounded toggles. When 'On', the track is Primary Blue; when 'Off', it is a hollow grey outline.
- **Search Bar:** Centered or docked top-right, featuring a subtle inner shadow and a magnifying glass glyph.