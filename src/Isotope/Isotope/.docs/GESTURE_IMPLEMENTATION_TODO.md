# MediaViewer Touch Gesture Implementation - TODO

## Overview
The MediaViewer component has been migrated from Vue 2 to Vue 3, but the touch gesture functionality (previously implemented with Hammer.js) needs to be re-implemented using Vue 3 compatible approaches.

## Current Status
- ✅ Component migrated to Vue 3
- ✅ Keyboard navigation working (Arrow keys, Escape)
- ✅ Desktop click navigation working
- ❌ Mobile touch gestures need implementation

## Required Gestures

### 1. Horizontal Swipe/Pan (High Priority)
**Purpose:** Navigate between images on mobile

**Original Implementation:**
```vue
<!-- Vue 2 / Hammer.js -->
<div v-hammer:swipe.left.right="handleHorizontalTouchEvents"
     v-hammer:pan="handleTouchEvents">
```

**Required Behavior:**
- Detect horizontal swipe/pan gestures
- Show preview of next/previous image during drag
- Snap to next/previous image if dragged far enough
- Animate back to current image if not dragged far enough
- Edge resistance when at first/last image (rubber band effect)
- Disable vertical scrolling during horizontal pan

**State to Manage:**
- `translateX` - Current horizontal translation
- `maxTranslateX` - Maximum translation during gesture
- `isTransitioning` - Whether animation is in progress
- `upcomingIndex` - Index of image to transition to

**Functions to Implement:**
- `handleHorizontalGestureMove(deltaX: number)`
- `handleHorizontalGestureEnd(deltaX: number)`
- `swipe(direction: number)`

---

### 2. Vertical Swipe/Pan Down (High Priority)
**Purpose:** Dismiss the viewer by swiping down

**Original Implementation:**
```vue
<!-- Vue 2 / Hammer.js -->
<div v-hammer:pan="handleTouchEvents">
```

**Required Behavior:**
- Detect vertical downward swipe/pan
- Translate viewer content vertically while dragging
- Fade background opacity during drag
- Close viewer if dragged down far enough (>50px)
- Animate back if not dragged far enough
- Only allow downward motion (no upward drag)

**State to Manage:**
- `translateY` - Current vertical translation
- `maxTranslateY` - Maximum translation during gesture
- `isClosing` - Whether viewer is being dismissed

**Functions to Implement:**
- `handleVerticalGestureMove(deltaY: number)`
- `handleVerticalGestureEnd(deltaY: number)`

---

### 3. Tap to Toggle Overlay (Medium Priority)
**Purpose:** Toggle UI overlay visibility on mobile

**Original Implementation:**
```vue
<!-- Vue 2 / Hammer.js -->
<div v-hammer:tap="onTap">
```

**Required Behavior:**
- Detect tap (quick touch and release)
- Distinguish between tap and drag start
- Toggle `isMobileOverlayVisible` state
- Ignore taps on interactive elements (links, tags)

**Current Implementation:**
```typescript
// Basic click handler - needs gesture refinement
function onTap(e: MouseEvent) {
  const target = e.target as HTMLElement;
  if (target.classList[0] !== 'overlay-tag' && target.tagName !== 'A' && isMobile.value) {
    isMobileOverlayVisible.value = !isMobileOverlayVisible.value;
  }
}
```

**Functions to Enhance:**
- `onTap(e: PointerEvent)` - Distinguish tap from drag

---

### 4. Unified Touch Event Handler (Critical)
**Purpose:** Route touch events to appropriate handlers based on direction

**Original Implementation:**
```typescript
handleTouchEvents(e: HammerInput) {
  if ((e.direction == 4 || 2 && Math.abs(e.deltaX) > 8) && this.translateY == 0) {
    this.handleHorizontalTouchEvents(e);
  } else if (e.direction == 8 || 16 && Math.abs(e.deltaY) > 8 && this.translateX == 0) {
    this.handleVerticalTouchEvents(e);
  }
}
```

**Required Behavior:**
- Detect initial touch direction
- Lock to horizontal or vertical based on initial movement
- Prevent simultaneous horizontal and vertical gestures
- Threshold detection (>8px movement)

**Direction Codes (Hammer.js reference):**
- 2: LEFT
- 4: RIGHT
- 8: UP
- 16: DOWN

**Functions to Implement:**
- `handleTouchEvents(e: PointerEvent)`

---

## Implementation Options

### Option 1: Native Pointer Events (Recommended)
**Pros:**
- No dependencies
- Modern browser API
- Full control over behavior
- Best performance

**Cons:**
- More code to write
- Need to handle edge cases manually

**Example:**
```typescript
let startX = 0;
let startY = 0;
let currentX = 0;
let currentY = 0;
let direction: 'horizontal' | 'vertical' | null = null;

function handlePointerDown(e: PointerEvent) {
  startX = e.clientX;
  startY = e.clientY;
  direction = null;
}

function handlePointerMove(e: PointerEvent) {
  const deltaX = e.clientX - startX;
  const deltaY = e.clientY - startY;

  // Determine direction on first significant movement
  if (!direction && (Math.abs(deltaX) > 8 || Math.abs(deltaY) > 8)) {
    direction = Math.abs(deltaX) > Math.abs(deltaY) ? 'horizontal' : 'vertical';
  }

  if (direction === 'horizontal' && translateY.value === 0) {
    handleHorizontalGestureMove(deltaX);
  } else if (direction === 'vertical' && translateX.value === 0 && deltaY > 0) {
    handleVerticalGestureMove(deltaY);
  }
}

function handlePointerUp(e: PointerEvent) {
  const deltaX = e.clientX - startX;
  const deltaY = e.clientY - startY;

  if (direction === 'horizontal') {
    handleHorizontalGestureEnd(deltaX);
  } else if (direction === 'vertical') {
    handleVerticalGestureEnd(deltaY);
  }

  direction = null;
}
```

---

### Option 2: VueUse Composables
**Pros:**
- Simple API
- Well-tested
- Part of Vue ecosystem

**Cons:**
- Additional dependency
- Less fine-grained control

**Example:**
```typescript
import { useSwipe, usePointer } from '@vueuse/core'

const mediaViewerContent = ref<HTMLElement | null>(null);

const { direction, lengthX, lengthY } = useSwipe(mediaViewerContent, {
  onSwipeEnd(e, direction) {
    if (direction === 'left') nav(1);
    if (direction === 'right') nav(-1);
    if (direction === 'down' && lengthY.value > 50) hide();
  }
});
```

---

### Option 3: Vue3 Touch Events
**Library:** `vue3-touch-events`

**Pros:**
- Drop-in replacement for v-hammer
- Familiar API
- Easy migration

**Cons:**
- External dependency
- Less popular/maintained

**Example:**
```typescript
// Install: npm install vue3-touch-events
import Vue3TouchEvents from 'vue3-touch-events'

// In template:
<div v-touch:swipe.left="() => nav(1)"
     v-touch:swipe.right="() => nav(-1)"
     v-touch:swipe.down="hide">
```

---

## Recommended Implementation Plan

### Phase 1: Setup (30 min)
1. Choose implementation approach (recommend Option 1 or 2)
2. Install dependencies if needed
3. Set up type definitions

### Phase 2: Horizontal Gestures (2-3 hours)
1. Implement basic left/right swipe
2. Add pan/drag preview
3. Add edge resistance
4. Test with different devices/browsers
5. Fine-tune thresholds and animations

### Phase 3: Vertical Gestures (1-2 hours)
1. Implement swipe-down to dismiss
2. Add background fade effect
3. Add snap-back animation
4. Test interactions with horizontal gestures

### Phase 4: Tap Handling (30 min)
1. Enhance tap detection
2. Prevent tap during drag
3. Test on various touch devices

### Phase 5: Testing & Polish (1-2 hours)
1. Test on iOS Safari
2. Test on Android Chrome
3. Test on tablets
4. Test edge cases (rapid gestures, multi-touch)
5. Adjust timing and thresholds
6. Add haptic feedback (optional)

**Total Estimated Time:** 4-6 hours

---

## Testing Checklist

### Horizontal Swipe
- [ ] Swipe left shows next image
- [ ] Swipe right shows previous image
- [ ] Partial swipe returns to current
- [ ] Edge resistance at first image (swipe right)
- [ ] Edge resistance at last image (swipe left)
- [ ] Smooth animation
- [ ] No jank or lag
- [ ] Works on iOS Safari
- [ ] Works on Android Chrome
- [ ] Works on tablets

### Vertical Swipe
- [ ] Swipe down dismisses viewer
- [ ] Background fades during drag
- [ ] Partial swipe returns to viewer
- [ ] Upward swipe is ignored
- [ ] No conflict with horizontal swipe
- [ ] Smooth animation
- [ ] Works on iOS Safari
- [ ] Works on Android Chrome

### Tap
- [ ] Tap toggles overlay
- [ ] Tap on link doesn't toggle overlay
- [ ] Tap on tag doesn't toggle overlay
- [ ] Drag doesn't trigger tap
- [ ] Works consistently

### Cross-Device
- [ ] iPhone (various sizes)
- [ ] Android phone (various sizes)
- [ ] iPad
- [ ] Android tablet
- [ ] Desktop touch screen

---

## Performance Considerations

1. **Use CSS transforms** for smooth 60fps animations
   - Already implemented: `transform: translate()`
   - Uses GPU acceleration

2. **Prevent layout thrashing**
   - Batch DOM reads and writes
   - Use `requestAnimationFrame` for smooth updates

3. **Passive event listeners**
   ```typescript
   element.addEventListener('touchstart', handler, { passive: true });
   ```

4. **Debounce/throttle where appropriate**
   - Already implemented for resize handler
   - Consider for pan events if needed

---

## Files to Modify

**Primary:**
- `Areas/Front/ClientAppVue3/source/components/content/MediaViewer.vue`

**Supporting (if needed):**
- `Areas/Front/ClientAppVue3/source/composables/useGestures.ts` (new file, optional)

---

## Code Locations

All TODO comments are in:
```
D:\sources\isotope\src\Isotope\Isotope\Areas\Front\ClientAppVue3\source\components\content\MediaViewer.vue
```

Search for:
- `// TODO: Implement touch gestures`
- `// TODO: Implement horizontal`
- `// TODO: Implement vertical`
- `console.warn('Touch gestures not yet implemented')`

---

## References

**Native APIs:**
- [Pointer Events API](https://developer.mozilla.org/en-US/docs/Web/API/Pointer_events)
- [Touch Events API](https://developer.mozilla.org/en-US/docs/Web/API/Touch_events)

**Libraries:**
- [VueUse - useSwipe](https://vueuse.org/core/useSwipe/)
- [VueUse - usePointer](https://vueuse.org/core/usePointer/)
- [vue3-touch-events](https://www.npmjs.com/package/vue3-touch-events)

**Previous Implementation:**
- [Hammer.js](https://hammerjs.github.io/)
- [vue2-hammer](https://www.npmjs.com/package/vue2-hammer)

---

## Support

If you encounter issues during implementation:
1. Check browser DevTools for touch event logs
2. Test with Chrome DevTools mobile emulation
3. Use `pointer-events: none` to debug touch target issues
4. Add visual debug indicators for deltaX/deltaY values
5. Test on real devices, not just emulators

---

**Priority:** Medium-High
**Blocking:** Mobile user experience
**Estimated Effort:** 4-6 hours
**Difficulty:** Medium
