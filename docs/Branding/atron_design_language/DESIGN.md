---
name: Atron Design Language
colors:
  surface: '#13131a'
  surface-dim: '#13131a'
  surface-bright: '#393840'
  surface-container-lowest: '#0e0e14'
  surface-container-low: '#1b1b22'
  surface-container: '#1f1f26'
  surface-container-high: '#2a2931'
  surface-container-highest: '#35343c'
  on-surface: '#e4e1eb'
  on-surface-variant: '#c7c4d7'
  inverse-surface: '#e4e1eb'
  inverse-on-surface: '#303037'
  outline: '#908fa0'
  outline-variant: '#464554'
  surface-tint: '#c0c1ff'
  primary: '#c0c1ff'
  on-primary: '#1000a9'
  primary-container: '#8083ff'
  on-primary-container: '#0d0096'
  inverse-primary: '#494bd6'
  secondary: '#bdc2ff'
  on-secondary: '#131e8c'
  secondary-container: '#2f3aa3'
  on-secondary-container: '#a8afff'
  tertiary: '#cebdff'
  on-tertiary: '#381385'
  tertiary-container: '#9b7fed'
  on-tertiary-container: '#31057e'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#e1e0ff'
  primary-fixed-dim: '#c0c1ff'
  on-primary-fixed: '#07006c'
  on-primary-fixed-variant: '#2f2ebe'
  secondary-fixed: '#e0e0ff'
  secondary-fixed-dim: '#bdc2ff'
  on-secondary-fixed: '#000767'
  on-secondary-fixed-variant: '#2f3aa3'
  tertiary-fixed: '#e8ddff'
  tertiary-fixed-dim: '#cebdff'
  on-tertiary-fixed: '#21005e'
  on-tertiary-fixed-variant: '#4f319c'
  background: '#13131a'
  on-background: '#e4e1eb'
  surface-variant: '#35343c'
typography:
  display-lg:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 48px
    fontWeight: '600'
    lineHeight: '1.1'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.2'
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.2'
  headline-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 24px
    fontWeight: '500'
    lineHeight: '1.3'
  body-lg:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.5'
  label-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 14px
    fontWeight: '500'
    lineHeight: '1.4'
    letterSpacing: 0.01em
  label-sm:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1.2'
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 0.25rem
  sm: 0.5rem
  md: 1rem
  lg: 1.5rem
  xl: 2.5rem
  container-margin: 2rem
  gutter: 1.5rem
---

## Brand & Style

The design system is engineered for the next generation of professional enterprise tools. It targets small to medium-sized companies that value precision, technical reliability, and a forward-thinking aesthetic. The brand personality is **professional, technological, and refined**, moving away from standard corporate palettes toward a deep-space, high-fidelity environment.

The visual direction follows a **Modern Glassmorphic** style. It utilizes deep, dark surfaces contrasted with luminous indigo accents and subtle "glow" states to establish a sense of depth and hierarchy. The interface should feel like a high-end dashboard: immersive yet strictly utilitarian, avoiding unnecessary ornamentation in favor of crisp borders, translucent layers, and vibrant backdrop blurs.

## Colors

This design system utilizes a dark-first color strategy to reduce eye strain and emphasize primary action areas. The palette is built on a foundation of "Deep Empty Refined" black, providing a high-contrast backdrop for the "Modern Indigo" primary tones.

- **Primary & Accents:** Indigo and Lavender are used exclusively for interactive elements, data visualization highlights, and primary branding moments.
- **Glassmorphism:** Surfaces are not solid; they use a semi-transparent `card_bg` with a subtle `card_border` to create a layered, multi-dimensional workspace.
- **Luminance:** Use `primary_glow` for subtle hover states or background decorative elements to reinforce the technical, high-energy feel of the platform.

## Typography

The design system employs **sans-serif system fonts** for clarity, interface familiarity, and operational readability. The approved stack is `Segoe UI, Arial, Helvetica, sans-serif`, with Segoe UI preferred on Windows. The typographic scale is designed for high-density information environments.

- **Headlines:** Use semi-bold weights with neutral letter-spacing to create a strong interface hierarchy without reducing readability.
- **Body:** Standardized at 16px for readability, using the "Muted Gray" color for long-form content to ensure a soft but legible contrast.
- **Labels:** Small caps or uppercase treatments are reserved for metadata and micro-copy, providing clear distinction from body text.
- **Restrictions:** Do not use serif, decorative, condensed, or highly stylized typefaces in product screens. Monospace is reserved only for code, logs, identifiers, and technical tabular values.

## Layout & Spacing

This design system uses a **Fluid Grid** model with a base-4 spacing rhythm. 

- **Grid:** A 12-column grid is used for desktop layouts, transitioning to an 8-column grid for tablets and a 4-column grid for mobile.
- **Margins:** Consistent 32px (2rem) outer margins ensure content is framed elegantly across all device types.
- **Reflow:** Cards and data tables should prioritize horizontal space on desktop, collapsing into stacked vertical cards on mobile viewports.
- **Density:** Maintain "High Contrast and Professional Spacing" by allowing generous vertical padding between sections to prevent the dark UI from feeling cramped.

## Elevation & Depth

Hierarchy is established through **Glassmorphism** and tonal stacking rather than traditional heavy shadows.

- **Base Layer:** The deepest background (#06060c) acts as the canvas.
- **Surface Layer:** Cards and Modals use a backdrop-filter (`blur(12px)`) with a semi-transparent indigo border. This creates a sense of floating above the base.
- **Glow Elevation:** Instead of black shadows, use `primary_glow_strong` (rgba) as a soft outer drop shadow for active or "elevated" elements like open dropdowns or primary buttons. This simulates a light-emitting technical display.

## Shapes

The design system uses **Rounded (0.5rem)** corners to balance the technical sans-serif typography with a touch of modern approachability.

- **Standard Components:** Buttons, Input fields, and Chips use the base 0.5rem radius.
- **Containers:** Larger surfaces like Cards use `rounded-lg` (1rem) to create a clear container-child relationship.
- **Icons:** Use thin, 1.5px stroke weights with squared-off ends to maintain the technological edge of the brand.

## Components

- **Buttons:** Primary buttons use a solid Modern Indigo fill. Secondary buttons are "ghost" style with a 1px Indigo border and a 10% Indigo background on hover.
- **Inputs:** Dark Secondary (#0c0c16) background with a subtle indigo border. Focused states should trigger a `primary_glow_strong` outer ring.
- **Cards:** Must utilize `backdrop-filter: blur()`. Borders are essential to define card edges against the deep dark background.
- **Chips:** Small, pill-shaped markers with a low-opacity Indigo background and high-contrast Indigo text for status or tagging.
- **Lists:** Rows should be separated by a 1px border using the `card_border` color, with a subtle hover state that increases background opacity to 100% of the Secondary Background color.
- **Navigation:** Vertical sidebars are preferred for this platform, using a glassmorphic blur to allow background gradients or glows to peek through.
