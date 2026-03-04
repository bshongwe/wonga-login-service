/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  eslint: {
    // Run ESLint on these directories during builds
    dirs: ['pages', 'components', 'lib', 'app', 'src'],
  },
  typescript: {
    // Type checking is already done by `tsc` in CI/CD
    // Set to true if you want to ignore type errors during builds
    ignoreBuildErrors: false,
  },
};

export default nextConfig;