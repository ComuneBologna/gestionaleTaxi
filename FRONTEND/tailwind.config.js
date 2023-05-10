const path = require('path');
const colors = require('tailwindcss/colors');
const defaultTheme = require('tailwindcss/defaultTheme');
const generatePalette = require(path.resolve(__dirname, ('node_modules/@asf/ng14-library/assets/tailwind/utils/generate-palette')));
const getConfig = require(path.resolve(__dirname, ('node_modules/@asf/ng14-library/assets/tailwind/tailwind.config')));

/**
 * Custom palettes
 *
 * Uses the generatePalette helper method to generate
 * Tailwind-like color palettes automatically
 */
const customPalettes = {

    primary: generatePalette("#0d47a1"),
    brand: generatePalette("#a26e7f")
};

/**
 * Themes
 */
const themes = {
    // Default theme is required for theming system to work correctly
    'default': {
        primary: {
            ...customPalettes.primary,
            DEFAULT: customPalettes.primary[500]
        },
        accent: {
            ...colors.slate,
            DEFAULT: colors.slate[600]
        },
        warn: {
            ...colors.red,
            DEFAULT: colors.red[600]
        },
        'on-warn': {
            500: colors.red['500']
        }
    },
    // Rest of the themes will use the 'default' as the base theme
    // and extend them with their given configuration
    'brand': {
        primary: customPalettes.brand
    },
    'teal': {
        primary: {
            ...colors.teal,
            DEFAULT: colors.teal[600]
        }
    },
    'rose': {
        primary: colors.rose
    },
    'purple': {
        primary: {
            ...colors.purple,
            DEFAULT: colors.purple[600]
        }
    },
    'amber': {
        primary: colors.amber
    }
};

/**
 * Tailwind configuration
 *
 * @param isProd
 * This will be automatically supplied by the custom Angular builder
 * based on the current environment of the application (prod, dev etc.)
 */
const config = getConfig(themes, defaultTheme, colors, path);
module.exports = config;