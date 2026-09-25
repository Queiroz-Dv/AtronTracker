---
name: Atron Light
colors:
  surface: '#faf8ff'
  surface-dim: '#d2d9f4'
  surface-bright: '#faf8ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f3ff'
  surface-container: '#eaedff'
  surface-container-high: '#e2e7ff'
  surface-container-highest: '#dae2fd'
  on-surface: '#131b2e'
  on-surface-variant: '#464554'
  inverse-surface: '#283044'
  inverse-on-surface: '#eef0ff'
  outline: '#767586'
  outline-variant: '#c7c4d7'
  surface-tint: '#494bd6'
  primary: '#4648d4'
  on-primary: '#ffffff'
  primary-container: '#6063ee'
  on-primary-container: '#fffbff'
  inverse-primary: '#c0c1ff'
  secondary: '#5b598c'
  on-secondary: '#ffffff'
  secondary-container: '#c7c3fe'
  on-secondary-container: '#514f81'
  tertiary: '#904900'
  on-tertiary: '#ffffff'
  tertiary-container: '#b55d00'
  on-tertiary-container: '#fffbff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#e1e0ff'
  primary-fixed-dim: '#c0c1ff'
  on-primary-fixed: '#07006c'
  on-primary-fixed-variant: '#2f2ebe'
  secondary-fixed: '#e3dfff'
  secondary-fixed-dim: '#c4c1fb'
  on-secondary-fixed: '#181445'
  on-secondary-fixed-variant: '#444173'
  tertiary-fixed: '#ffdcc5'
  tertiary-fixed-dim: '#ffb783'
  on-tertiary-fixed: '#301400'
  on-tertiary-fixed-variant: '#703700'
  background: '#faf8ff'
  on-background: '#131b2e'
  surface-variant: '#dae2fd'
typography:
  display:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.1'
    letterSpacing: -0.04em
  headline-lg:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg-mobile:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.3'
    letterSpacing: -0.01em
  body-lg:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
    letterSpacing: '0'
  body-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.5'
    letterSpacing: '0'
  body-sm:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
    letterSpacing: '0'
  label-md:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 14px
    fontWeight: '500'
    lineHeight: '1'
    letterSpacing: 0.02em
  label-sm:
    fontFamily: Segoe UI, Arial, Helvetica, sans-serif
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 40px
  2xl: 64px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 48px
---

## Brand & Style

The design system is a high-contrast, professional framework built for technical clarity and modern efficiency. It evolves the previous aesthetic into a crisp, light-mode environment that prioritizes legibility and structural discipline.

The style is **Modern Corporate Minimalism** with a focus on precision. It utilizes generous whitespace, deep black typography, and a vibrant indigo primary accent to create a high-energy yet professional workspace. The emotional response should be one of reliability, speed, and technical sophistication, making it ideal for developer tools, SaaS platforms, and data-intensive applications.

## Colors

The palette is anchored by a stark white background to maximize contrast and "breathability." 

- **Primary:** Modern Indigo (#6366F1) serves as the singular action color, used for primary buttons, active states, and critical focal points.
- **Neutrals:** A scale of deep slate and zinc grays provides hierarchy. Text uses #0F172A (Deep Black/Blue) for maximum readability against white.
- **Accents:** Secondary actions utilize #1E1B4B (Indigo Dye) for subtle differentiation from the primary brand color.
- **Semantic:** Success, Warning, and Error colors should remain high-chroma but follow the light-theme accessible contrast ratios (WCAG AAA preferred for text).

## Typography

This design system exclusively uses **sans-serif system fonts**, prioritizing Segoe UI on Windows and falling back to Arial, Helvetica, and the generic sans-serif family. The goal is direct legibility in operational screens, data grids, dense forms, and long work sessions.

- **Headlines:** Use Bold and Semi-Bold weights with neutral letter-spacing. Avoid decorative, serif, condensed, or highly stylized fonts.
- **Body:** Standard body text is set in Regular weight with generous line-height to maintain legibility during long reading sessions.
- **Font Stack:** `Segoe UI, Arial, Helvetica, sans-serif` is the approved stack for the application and should be reinforced in global CSS, Angular Material typography, and component-level overrides when needed.
- **Monospace:** Use monospace only for code snippets, identifiers, logs, or tabular technical values. Do not use monospace for body text or navigation.
- **Contrast:** Ensure all text on white backgrounds maintains at least a 4.5:1 ratio, though 7:1 is preferred for primary body content.

## Layout & Spacing

The design system employs a **12-column fluid grid** for desktop and a **4-column grid** for mobile. 

- **The 4px Rule:** All spacing and sizing must be a multiple of 4px. This maintains the "technical" feel and ensures alignment across complex dashboards.
- **Containers:** Content is typically housed in centered containers with a max-width of 1280px.
- **Density:** Use "Comfortable" spacing for marketing/landing pages and "Compact" spacing (8px/12px) for application sidebars and data grids.

## Elevation & Depth

To maintain a clean, high-contrast look, this design system moves away from heavy shadows in favor of **Tonal Layers** and **Crisp Outlines**.

- **Level 0 (Base):** #FFFFFF.
- **Level 1 (Card/Surface):** #F8FAFC with a 1px solid border (#E2E8F0).
- **Interactive Elevation:** Only use shadows to indicate temporary state changes (e.g., a dropdown menu or a dragged item). These shadows should be "Ambient" — highly diffused, low opacity (4-8%), and neutral-tinted.
- **Separation:** Horizontal and vertical rules should be #F1F5F9 (very light gray) to provide structure without cluttering the visual field.

## Shapes

In alignment with the "Round Eight" philosophy, the design system utilizes a **0.5rem (8px)** base radius.

- **Base (Buttons, Inputs):** 8px (0.5rem).
- **Large (Cards, Modals):** 16px (1rem).
- **Extra Large (Feature Sections):** 24px (1.5rem).

Avoid full pills (circular ends) except for status indicators (Chips). The 8px radius provides a modern, friendly touch while maintaining the structural integrity of the grid.

## Components

- **Buttons:** Primary buttons are #6366F1 with White text. Secondary buttons use a White background with a 1px #E2E8F0 border and Deep Black (#0F172A) text.
- **Input Fields:** Use #FFFFFF background with a 1px #E2E8F0 border. On focus, the border transitions to #6366F1 with a 2px subtle indigo outer glow.
- **Chips:** Small, 100px roundedness (pill), using light tinted backgrounds (e.g., Primary at 10% opacity) with the full-chroma text color inside.
- **Cards:** White background, 1px border (#E2E8F0), and an 8px corner radius. No shadow by default.
- **Icons:** Use the "Deep Purple" variant of the primary color or pure black. Stroke weights should be consistent (1.5px or 2px) to match the sans-serif typography weight.
- **Lists:** Clean rows with 1px bottom borders. Hover states should use #F8FAFC (Surface color) to indicate interactivity.
