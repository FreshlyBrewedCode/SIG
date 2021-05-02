module.exports = {
    tagFormat: "v${version}",
    plugins: [
        ["@semantic-release/commit-analyzer", { preset: "angular" }],
        "@semantic-release/release-notes-generator",
        [
            "@semantic-release/changelog",
            {
                preset: "angular",
                changelogFile: process.env.pkgRoot + "/CHANGELOG.md",
            },
        ],
        [
            "@semantic-release/npm",
            {
                npmPublish: false,
                pkgRoot: process.env.pkgRoot,
            },
        ],
        [
            "@semantic-release/git",
            {
                assets: [
                    process.env.pkgRoot + "/package.json",
                    process.env.pkgRoot + "/CHANGELOG.md",
                ],
                message:
                    "chore(release): ${nextRelease.version} [skip ci]\n\n${nextRelease.notes}",
            },
        ],
        "@semantic-release/github",
    ],
};
