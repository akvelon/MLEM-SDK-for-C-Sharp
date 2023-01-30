from sklearn.linear_model import LogisticRegression
from mlem.api import save
import pandas as pd

url = 'http://bit.ly/kaggletrain'
train = pd.read_csv(url)

feature_cols = ['Pclass', 'Parch']
X = train.loc[:, feature_cols]

y = train.Survived

logreg = LogisticRegression()
logreg.fit(X, y)

save(
    logreg,
    sample_data=X,
    path="scikit_mlem",
)