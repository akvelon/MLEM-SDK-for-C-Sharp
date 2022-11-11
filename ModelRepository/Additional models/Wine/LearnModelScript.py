# train.py
from sklearn.datasets import load_wine
from sklearn.ensemble import RandomForestClassifier

from mlem.api import save


def main():
    data, y = load_wine(return_X_y=True, as_frame=True)
    rf = RandomForestClassifier(
        n_jobs=2,
        random_state=42,
    )
    rf.fit(data, y)

    save(
        rf,
        "rf_wine",
        sample_data=data,
    )


if __name__ == "__main__":
    main()
